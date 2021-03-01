using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] Model model;
    [SerializeField] View view;

    [SerializeField] private int score = 0;
    [SerializeField] private int turns = 0;
    [SerializeField] private int minCoinsToChain = 0;
    [SerializeField] private List<Coin.COIN_TYPE> coin_typesAllowed = null;

    public AudioSource musicAudioSource;
    public AudioSource SFXAudioSource;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    [SerializeField] private List<GameObject> chainedCoins = null;
    [SerializeField] private List<GameObject> newSpawnedCoins = null;
    [SerializeField] private GameObject selectedCoin = null;

    private List<Vector2> positions = null;

    public event Action<int> OnTurnUpdate = delegate { };
    public event Action<int> OnScoreUpdate = delegate { };
    [HideInInspector] public bool isRestartingGame = false;

    private enum GAME_STATE
    {
        IDLE,
        COMBINING,
        DROPPING
    }
    private GAME_STATE game_state = GAME_STATE.IDLE;

    private Coin.COIN_TYPE coinTypeToChain;
    private int[,] grid = null;
    private int auxTurns;
    private Coin[,] gridCoins = null;
    private Vector2[,] gridPositions = null;
    private Camera cam;

    private int maxX = 0;
    private int maxY = 0;

    void Start()
    {
        grid = model.GetGrid(ref maxX, ref maxY);
        gridCoins = new Coin[maxX, maxY];

        cam = Camera.main;

        chainedCoins = new List<GameObject>();
        positions = new List<Vector2>();
        auxTurns = turns;

        musicAudioSource.pitch = 1;
        OnTurnUpdate(turns);

        InitGrid();

        float coinRes = Screen.currentResolution.width * Mathf.Max(maxX, maxY);
        cam.orthographicSize = 9 / (115200f / coinRes) * 9;
        cam.transform.position = new Vector3((maxX - 1) * view.CoinOffset.x / 2, (maxY - 1) * view.CoinOffset.y / 2, cam.transform.position.z);
    }

    private void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (turns <= 0)
        {
            StartCoroutine(RestartGame());
            return;
        }

        float turnsPercentage = ((float)turns / (float)auxTurns) * 100f;

        if (turnsPercentage <= 25f)
        {
            musicAudioSource.pitch = Mathf.Lerp(musicAudioSource.pitch, 1.2f, 2 * Time.deltaTime);
        }

        switch (game_state)
        {

            case GAME_STATE.IDLE:

                if (Input.GetMouseButtonDown(0))
                {
                    if (hit)
                    {
                        view.AddLine(hit.transform.position + Vector3.back);

                        positions.Add(hit.transform.position);
                        chainedCoins.Add(hit.transform.gameObject);

                        selectedCoin = chainedCoins[chainedCoins.Count - 1];
                        coinTypeToChain = selectedCoin.GetComponent<Coin>().Coin_type;
                        game_state = GAME_STATE.COMBINING;
                    }
                }
                break;

            case GAME_STATE.COMBINING:

                if (hit)
                {
                    if (!chainedCoins.Contains(hit.transform.gameObject))
                    {
                        if (hit.transform.gameObject.GetComponent<Coin>().Coin_type == coinTypeToChain)
                        {
                            if (model.CanMoveToPosition(hit.transform.gameObject, selectedCoin, view.CoinOffset))
                            {
                                view.AddLine(new Vector3(hit.transform.position.x, hit.transform.position.y, -1));
                                chainedCoins.Add(hit.transform.gameObject);
                                positions.Add(hit.transform.position);
                                selectedCoin = chainedCoins[chainedCoins.Count - 1];
                            }
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    CancelChaining();
                    game_state = GAME_STATE.IDLE;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (chainedCoins.Count >= minCoinsToChain)
                    {
                        game_state = GAME_STATE.DROPPING;
                    }
                    else
                    {
                        SFXAudioSource.PlayOneShot(wrongSFX, 0.5f);
                        RefreshChain();
                        game_state = GAME_STATE.IDLE;
                    }
                    view.ClearLine();
                }
                break;

            case GAME_STATE.DROPPING:

                score += 10 * chainedCoins.Count;
                SFXAudioSource.PlayOneShot(correctSFX, 0.4f);

                for (int i = 0; i < chainedCoins.Count; i++)
                {
                    Destroy(chainedCoins[i]);
                }
                SpawnFallingNewCoins(positions);
                turns--;

                OnScoreUpdate(score);
                OnTurnUpdate(turns);

                RefreshChain();
                game_state = GAME_STATE.IDLE;
                break;
        }
    }

    void InitGrid()
    {
        gridCoins = new Coin[maxX, maxY];
        gridPositions = new Vector2[maxX, maxY];
        chainedCoins = new List<GameObject>();
        positions = new List<Vector2>();

        StartCoroutine(CreateCoins());
    }

    void SpawnFallingNewCoins(List<Vector2> positions)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            newSpawnedCoins.Add(view.CreateCoin(new Vector3(positions[i].x, positions[i].y + (1.5f * maxY)), coin_typesAllowed).gameObject);
        }
    }

    void RefreshChain()
    {
        for (int i = newSpawnedCoins.Count - 1; i > 0; i--)
        {
            if (newSpawnedCoins[i] == null)
            {
                newSpawnedCoins.RemoveAt(i);
            }
        }
        chainedCoins.Clear();
        positions.Clear();
    }

    IEnumerator CreateCoins()
    {
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                gridCoins[i, j] = view.CreateCoin(new Vector3(view.CoinOffset.x * i, view.CoinOffset.y * j, 0), coin_typesAllowed);
                gridPositions[i, j] = gridCoins[i, j].transform.position;
                yield return null;
            }
        }
    }

    void CancelChaining()
    {
        view.ClearLine();
        chainedCoins.Clear();
        positions.Clear();
    }

    public IEnumerator RestartGame()
    {
        turns = auxTurns;
        isRestartingGame = true;
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                if (gridCoins[i, j] != null)
                {
                    Destroy(gridCoins[i, j].gameObject);
                }
            }
        }

        for (int i = 0; i < newSpawnedCoins.Count; i++)
        {
            Destroy(newSpawnedCoins[i]);
        }
        newSpawnedCoins.Clear();

        yield return new WaitForSeconds(2f);

        score = 0;
        musicAudioSource.pitch = 1;
        OnScoreUpdate(score);
        OnTurnUpdate(turns);

        InitGrid();
        isRestartingGame = false;
    }
}
