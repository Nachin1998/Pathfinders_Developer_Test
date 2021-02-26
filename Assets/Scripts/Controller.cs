using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] Model model;
    [SerializeField] View view;

    [SerializeField] private int maxSizeX = 0;
    [SerializeField] private int maxSizeY = 0;

    [SerializeField] private int score = 0;
    [SerializeField] private int turns = 0;
    [SerializeField] private int maxCoinsToChain = 0;
    [SerializeField] private List<Coin.COIN_TYPE> coin_typesAllowed = null;
    
    public AudioSource musicAudioSource;
    public AudioSource SFXAudioSource;
    public AudioClip wrongSFX;

    [SerializeField] private List <GameObject> chainedCoins = null;
    [SerializeField] private List <GameObject> newSpawnedCoins = null;
    private GameObject selectedCoin = null;

    private List <Vector2> positions = null;

    public event Action <int> OnTurnUpdate = delegate { };
    public event Action <int> OnScoreUpdate = delegate { };

    private enum GAME_STATE
    {
        IDLE,
        COMBINING
    }
    private GAME_STATE game_state = GAME_STATE.IDLE;

    private Coin.COIN_TYPE coinTypeToChain;
    //private int[,] grid = null;
    private int auxTurns;
    private Coin[,] gridCoins = null;
    private Camera cam;

    void Start()
    {
        //grid = new int[maxSizeX, maxSizeY];
        gridCoins = new Coin[maxSizeX, maxSizeY];
        cam = Camera.main;
        chainedCoins = new List<GameObject>();
        positions = new List<Vector2>();
        auxTurns = turns;

        musicAudioSource.pitch = 1;
        OnTurnUpdate(turns);

        InitGrid();

        float coinRes = Screen.currentResolution.width * Mathf.Max(maxSizeX, maxSizeY);
        cam.orthographicSize = 9 / (115200f / coinRes) * 9;
        cam.transform.position = new Vector3((maxSizeX - 1) * view.CoinOffset.x / 2, (maxSizeY - 1) * view.CoinOffset.y / 2, cam.transform.position.z);
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
                        view.AddLine(hit.transform.position);

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
                    if (hit.transform.gameObject.GetComponent<Coin>().Coin_type == coinTypeToChain)
                    {
                        if (model.CanMoveToPosition(hit.transform.gameObject, selectedCoin, view.CoinOffset))
                        {
                            if (!chainedCoins.Contains(hit.transform.gameObject))
                            {
                                view.AddLine(new Vector3(hit.transform.position.x, hit.transform.position.y, -1));
                                chainedCoins.Add(hit.transform.gameObject);
                                positions.Add(hit.transform.position);
                                selectedCoin = chainedCoins[chainedCoins.Count - 1];
                            }
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (chainedCoins.Count >= maxCoinsToChain)
                    {
                        score += 10 * chainedCoins.Count;

                        for (int i = 0; i < chainedCoins.Count; i++)
                        {
                            Destroy(chainedCoins[i]);
                        }

                        for (int i = 0; i < positions.Count; i++)
                        {
                            newSpawnedCoins.Add(view.CreateCoin(positions[i], coin_typesAllowed).gameObject);
                        }

                        turns--;

                        OnScoreUpdate(score);
                        OnTurnUpdate(turns);

                        chainedCoins.Clear();
                        positions.Clear();
                        game_state = GAME_STATE.IDLE;
                    }
                    else
                    {
                        SFXAudioSource.PlayOneShot(wrongSFX, 0.5f);
                        chainedCoins.Clear();
                        positions.Clear();
                        game_state = GAME_STATE.IDLE;
                    }
                    view.ClearLine();
                }
                break;
        }
    }

    void InitGrid()
    {
        gridCoins = new Coin[maxSizeX, maxSizeY];
        chainedCoins = new List<GameObject>();
        positions = new List<Vector2>();

        StartCoroutine(CreateCoins());
    }
    IEnumerator CreateCoins()
    {
        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                gridCoins[i, j] = view.CreateCoin(new Vector3(view.CoinOffset.x * i, view.CoinOffset.y * j, 0), coin_typesAllowed);
                yield return null;
            }
        }
    }
    public IEnumerator RestartGame()
    {
        turns = auxTurns;

        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                if(gridCoins[i, j] != null)
                {
                    Destroy(gridCoins[i, j].gameObject);
                    //yield return new WaitForSeconds(0.1f);
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
    }
}
