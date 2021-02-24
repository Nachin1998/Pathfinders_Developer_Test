using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum COIN_TYPE
    {
        WATER,
        EARTH,
        FIRE,
        AIR
    }
    [SerializeField] private COIN_TYPE coin_type;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Animator animator;

    public COIN_TYPE Coin_type { get { return coin_type; } set { coin_type = value; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

}
