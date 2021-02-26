﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private Coin coinPrefab = null;
    [SerializeField] private List<Sprite> coinSprites = null;
    [SerializeField] private Vector2 coinOffset = Vector2.zero;
    [SerializeField] private Vector2 boxColliderSize = Vector2.zero;
    [SerializeField] private Transform parent = null;

    private int[] maxTypes;

    public Vector2 CoinOffset { get { return coinOffset; } }

    public Coin CreateCoin(Vector3 pos, List<Coin.COIN_TYPE> typesAllowed)
    {
        Coin go = Instantiate(coinPrefab, pos, Quaternion.identity, parent);        

        go.gameObject.name = "Coin";
        go.transform.parent = parent;
        go.transform.position = pos;

        maxTypes = new int[typesAllowed.Count];

        for (int i = 0; i < maxTypes.Length; i++)
        {
            maxTypes[i] = (int)typesAllowed[i];
        }

        int coinType = maxTypes[Random.Range(0, typesAllowed.Count)];
        go.SpriteRenderer.sprite = coinSprites[coinType];
        go.Coin_type = (Coin.COIN_TYPE)coinType;

        return go;
    }
}
