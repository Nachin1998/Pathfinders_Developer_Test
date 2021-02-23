using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] Model model;
    [SerializeField] View view;

    [SerializeField] private int maxSizeX = 0;
    [SerializeField] private int maxSizeY = 0;

    private int[,] grid = null;
    private Coin[,] gridCoins = null;

    void Start()
    {
        grid = new int[maxSizeX, maxSizeY];
        gridCoins = new Coin[maxSizeX, maxSizeY];

        int index = 0;
        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                gridCoins[i, j] = view.CreateCoin(index, new Vector3(view.TokenOffset.x * i, view.TokenOffset.y * j, 0));
                index++;
            }
        }
        Vector3 middleCoin = gridCoins[maxSizeX / 2, maxSizeY / 2].transform.position;

        if(maxSizeX % 2 == 0)
        {
            Camera.main.gameObject.transform.position = new Vector3(middleCoin.x - view.TokenOffset.x / 2, middleCoin.y - view.TokenOffset.y / 2, Camera.main.gameObject.transform.position.z);
        }
        else
        {
            Camera.main.gameObject.transform.position = new Vector3(middleCoin.x, middleCoin.y, Camera.main.gameObject.transform.position.z);
        }
    }
}
