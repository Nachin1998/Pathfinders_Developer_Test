using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    [SerializeField] private int maxSizeX = 0;
    [SerializeField] private int maxSizeY = 0;
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public List <GameObject> SelectCoin(GameObject coin, List<GameObject> currentSelectedCoinList)
    {
        if (!currentSelectedCoinList.Contains(coin))
        {
            currentSelectedCoinList.Add(coin);
        }

        return currentSelectedCoinList;
    }
}
