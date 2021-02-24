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
    [SerializeField] private List<GameObject> chainedCoins = null;
    private enum GAME_STATE
    {
        IDLE,
        COMBINING,
        EXPLODING
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

        int index = 0;
        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                gridCoins[i, j] = view.CreateCoin(index, new Vector3(view.CoinOffset.x * i, view.CoinOffset.y * j, 0));
                index++;
            }
        }

        Vector3 middleCoin = gridCoins[maxSizeX / 2, maxSizeY / 2].transform.position;
        if (maxSizeX % 2 == 0)
        {
            cam.gameObject.transform.position = new Vector3(middleCoin.x - view.CoinOffset.x / 2, middleCoin.y - view.CoinOffset.y / 2, cam.gameObject.transform.position.z);
        }
        else
        {
            cam.gameObject.transform.position = new Vector3(middleCoin.x, middleCoin.y, cam.gameObject.transform.position.z);
        }
    }

    private void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        switch (game_state)
        {
            case GAME_STATE.IDLE:

                if (hit)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedCoin = hit.transform.gameObject;
                        coinTypeToChain = selectedCoin.GetComponent<Coin>().Coin_type;
                        chainedCoins.Add(selectedCoin);
                        game_state = GAME_STATE.COMBINING;
                    }
                }                
                break;

            case GAME_STATE.COMBINING:                

                if (hit)
                {
                    if (hit.transform.gameObject != selectedCoin)
                    {
                        if (hit.transform.gameObject.GetComponent<Coin>().Coin_type == coinTypeToChain)
                        {
                            if (hit.transform.position == new Vector3(selectedCoin.transform.position.x + view.CoinOffset.x, selectedCoin.transform.position.y) ||
                                hit.transform.position == new Vector3(selectedCoin.transform.position.x - view.CoinOffset.x, selectedCoin.transform.position.y))
                            {
                                chainedCoins.Add(hit.transform.gameObject);
                                Debug.Log(chainedCoins.Count);                                
                            }
                        }
                    }
                        
                    //coinTypeToChain = hit.transform.gameObject.GetComponent<Coin>().Coin_type;
                    //game_state = GAME_STATE.COMBINING;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if(chainedCoins.Count > maxCoinsToChain)
                    {
                        for (int i = 0; i < chainedCoins.Count; i++)
                        {
                            Destroy(chainedCoins[i]);
                            game_state = GAME_STATE.IDLE;
                        }
                    }                    
                }
                break;

            case GAME_STATE.EXPLODING:
                break;
        }
        
    }
}
