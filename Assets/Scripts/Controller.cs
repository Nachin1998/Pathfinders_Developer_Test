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
    [SerializeField] private int turnsRemaining = 0;
    [SerializeField] private int maxCoinsToChain = 0;
    [SerializeField] private GameObject selectedCoin = null;
    [SerializeField] private List <GameObject> chainedCoins = null;
    [SerializeField] private List <Coin.COIN_TYPE> coin_typesAllowed = null;

    private List <GameObject> extraChainedCoins = null;
    private List <Vector2> positions = null;
    
    public event Action <int> OnTurnUpdate = delegate { };
    public event Action <int> OnScoreUpdate = delegate { };

    private enum GAME_STATE
    {
        IDLE,
        COMBINING
    }
    [SerializeField] private GAME_STATE game_state = GAME_STATE.IDLE;

    private Coin.COIN_TYPE coinTypeToChain;
    private int[,] grid = null;
    private Coin[,] gridCoins = null;
    private Camera cam;

    void Start()
    {
        grid = new int[maxSizeX, maxSizeY];
        gridCoins = new Coin[maxSizeX, maxSizeY];
        cam = Camera.main;
        chainedCoins = new List<GameObject>();
        positions = new List<Vector2>();

        int index = 0;
        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                gridCoins[i, j] = view.CreateCoin(new Vector3(view.CoinOffset.x * i, view.CoinOffset.y * j, 0), coin_typesAllowed);
                index++;
            }
        }

        float coinRes = Screen.currentResolution.width * Mathf.Max(maxSizeX, maxSizeY);
        cam.orthographicSize = 9 / (115200f / coinRes) * 9;
        cam.transform.position = new Vector3((maxSizeX - 1) * view.CoinOffset.x / 2, (maxSizeY - 1) * view.CoinOffset.y / 2, cam.transform.position.z);
    }

    private void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (turnsRemaining <= 0)
        {
            StartCoroutine(RestartGame());
            
            return;
        }

        switch (game_state)
        {
            case GAME_STATE.IDLE:

                if (Input.GetMouseButtonDown(0))
                {
                    if (hit)
                    {
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
                        if (hit.transform.position.y == selectedCoin.transform.position.y + view.CoinOffset.y ||
                            hit.transform.position.y == selectedCoin.transform.position.y - view.CoinOffset.y)
                        {
                            if (!chainedCoins.Contains(hit.transform.gameObject))
                            {
                                chainedCoins.Add(hit.transform.gameObject);
                                positions.Add(hit.transform.position);
                            }
                            selectedCoin = chainedCoins[chainedCoins.Count - 1];
                        }

                        if (hit.transform.position.x == selectedCoin.transform.position.x + view.CoinOffset.x ||
                            hit.transform.position.x == selectedCoin.transform.position.x - view.CoinOffset.x)
                        {
                            if (!chainedCoins.Contains(hit.transform.gameObject))
                            {
                                chainedCoins.Add(hit.transform.gameObject);
                                positions.Add(hit.transform.position);
                            }
                            selectedCoin = chainedCoins[chainedCoins.Count - 1];
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (chainedCoins.Count > maxCoinsToChain)
                    {
                        score += 10 * chainedCoins.Count;
                        for (int i = 0; i < chainedCoins.Count; i++)
                        {
                            Destroy(chainedCoins[i]);
                            game_state = GAME_STATE.IDLE;
                        }

                        for (int i = 0; i < positions.Count; i++)
                        {
                            extraChainedCoins.Add(view.CreateCoin(positions[i], coin_typesAllowed).gameObject);
                        }

                        turnsRemaining--;
                        OnScoreUpdate(score);
                        OnTurnUpdate(turnsRemaining);
                        chainedCoins.Clear();
                        positions.Clear();
                    }
                    else
                    {
                        chainedCoins.Clear();
                        positions.Clear();
                        game_state = GAME_STATE.IDLE;
                    }
                }
                break;
        }
    }

    IEnumerator RestartGame()
    {
        turnsRemaining = 3;

        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                if(gridCoins[i, j] != null)
                {
                    Destroy(gridCoins[i, j].gameObject);
                }
            }
        }

        for (int i = 0; i < extraChainedCoins.Count; i++)
        {
            Destroy(extraChainedCoins[i]);
        }
        extraChainedCoins.Clear();

        yield return new WaitForSeconds(2f);

        OnTurnUpdate(turnsRemaining);
        gridCoins = new Coin[maxSizeX, maxSizeY];
        chainedCoins = new List<GameObject>();
        positions = new List<Vector2>();

        int index = 0;
        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                gridCoins[i, j] = view.CreateCoin(new Vector3(view.CoinOffset.x * i, view.CoinOffset.y * j, 0), coin_typesAllowed);
                index++;
            }
        }
    }
}
