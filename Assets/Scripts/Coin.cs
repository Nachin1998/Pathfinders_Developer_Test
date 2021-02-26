﻿using System.Collections;
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
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 startingPos;

    public Animator animator;
    public AudioSource audioSource;

    public COIN_TYPE Coin_type { get { return coin_type; } set { coin_type = value; } }
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } set { spriteRenderer = value; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        startingPos = transform.position;
    }    

    public void SpawnBubblePop()
    {
        audioSource.PlayOneShot(audioSource.clip, 0.5f);
    }

    private void OnMouseEnter()
    {
        animator.SetBool("OnMouseEnter", true);
        audioSource.PlayOneShot(audioSource.clip, 1f);
    }

    private void OnMouseExit()
    {
        animator.SetBool("OnMouseEnter", false);
    }
}
