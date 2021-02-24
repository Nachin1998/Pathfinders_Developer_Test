using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private Coin coin = null;
    [SerializeField] private List<Sprite> coinSprites = null;
    [SerializeField] private Vector2 coinOffset = Vector2.zero;
    [SerializeField] private Vector2 boxColliderSize = Vector2.zero;
    [SerializeField] private Transform parent = null;
    [SerializeField] private LineRenderer lineRenderer = null;

    public Vector2 CoinOffset { get { return coinOffset; } }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public Coin CreateCoin(Vector3 pos)
    {
        Coin go = Instantiate(coin, pos, Quaternion.identity, parent);

        go.gameObject.name = "Coin";
        go.transform.parent = parent;
        go.transform.position = pos;
        
        int coinType = Random.Range(0, coinSprites.Count);
        go.SpriteRenderer.sprite = coinSprites[coinType];
        go.Coin_type = (Coin.COIN_TYPE)coinType;

        return go;
    }

    public void StartOnMouseAnimation(Coin coin)
    {
        coin.animator.SetBool("OnMouseEnter", true);
    }

    public void StopOnMouseAnimation(Coin coin)
    {
        coin.animator.SetBool("OnMouseEnter", false);
    }
}
