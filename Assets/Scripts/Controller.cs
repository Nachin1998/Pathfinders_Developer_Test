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
    public AudioClip wrongSFX;

    [SerializeField] private List <GameObject> chainedCoins = null;
    [SerializeField] private List <GameObject> newSpawnedCoins = null;
    private GameObject selectedCoin = null;

    private List <Vector2> positions = null;

    public event Action <int> OnTurnUpdate = delegate { };
    public event Action <int> OnScoreUpdate = delegate { };
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

    int maxX = 0;
    int maxY = 0;

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
                    if (chainedCoins.Count >= minCoinsToChain)
                    {
                        score += 10 * chainedCoins.Count;

                        for (int i = 0; i < chainedCoins.Count; i++)
                        {
                            Destroy(chainedCoins[i]);
                        }

                        //for (int i = 0; i < positions.Count; i++)
                        //{
                        //    newSpawnedCoins.Add(view.CreateCoin(0, positions[i], coin_typesAllowed).gameObject);
                        //}

                        turns--;

                        
                        OnScoreUpdate(score);
                        OnTurnUpdate(turns);

                        chainedCoins.Clear();
                        positions.Clear();
                        
                        game_state = GAME_STATE.DROPPING;
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

            case GAME_STATE.DROPPING:
                //for (int i = 0; i < maxX; i++)
                //{
                //    for (int j = 0; j < maxY; j++)
                //    {
                //        if(gridCoins[i, j] != null)
                //        {
                //            if (gridCoins[i, j].transform.position.y != 0)
                //            {
                //                StartCoroutine(model.DropCoinDown(gridCoins[i, j].gameObject));
                //            }
                //        }
                //    }
                //}
                game_state = GAME_STATE.IDLE;
                break;
        }

        //for (int i = 0; i < maxX; i++)
        //{
        //    for (int j = 0; j < maxY; j++)
        //    {
        //        if (gridCoins[i, j] != null)
        //        {
        //            model.DropCoinDown(gridCoins[i, j].gameObject);
        //        }
        //    }
        //}
    }

    void InitGrid()
    {
        gridCoins = new Coin[maxX, maxY];
        gridPositions = new Vector2[maxX, maxY];
        chainedCoins = new List<GameObject>();
        positions = new List<Vector2>();

        //StartCoroutine(CreateCoins());
        CreateCoins();
    }

    void CreateCoins()
    {
        int index = 0;
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                gridCoins[i, j] = view.CreateCoin(index, new Vector3(view.CoinOffset.x * i, view.CoinOffset.y * j, 0), coin_typesAllowed);
                gridPositions[i, j] = gridCoins[i, j].transform.position;
                index++;
                //yield return null;
            }
        }
    }

    public IEnumerator RestartGame()
    {
        turns = auxTurns;
        isRestartingGame = true;
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
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
        isRestartingGame = false;
    }
}
