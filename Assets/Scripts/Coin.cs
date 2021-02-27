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
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D col2D;
    [SerializeField] private Vector2 startingPos;
    [SerializeField] private LayerMask layerMask;

    public Animator animator;
    public AudioSource audioSource;

    public COIN_TYPE Coin_type { get { return coin_type; } set { coin_type = value; } }
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } set { spriteRenderer = value; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (transform.position.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, 0);
            return;
        }

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position - new Vector3(0, 0.6f), new Vector2(0, -1.5f), 0.5f);
        Debug.DrawRay(transform.position - new Vector3(0, 0.6f), new Vector3(0, -1.5f));
        
        if (!hit2D)
        {
            transform.position += Vector3.down * 2 * Time.deltaTime;
        }
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
