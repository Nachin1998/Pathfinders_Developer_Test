using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum Coin_type
    {
        WATER,
        EARTH,
        FIRE,
        AIR
    }
    [SerializeField] public Coin_type coin_type;
    [SerializeField] public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

}
