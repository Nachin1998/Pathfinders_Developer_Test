using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private Coin coin = null;
    [SerializeField] private List<Sprite> coinSprites = null;
    [SerializeField] private Vector2 tokenOffset = Vector2.zero;
    [SerializeField] private Vector2 boxColliderSize = Vector2.zero;
    [SerializeField] private Transform parent = null;

    public Vector2 TokenOffset { get { return tokenOffset; } }

    void Start()
    {
        
    }

    public Coin CreateCoin(int index, Vector3 pos)
    {
        Coin go = Instantiate(coin, pos, Quaternion.identity, parent);
        go.gameObject.name = "Coin " + index;
        go.transform.parent = parent;
        go.transform.position = pos;
        
        int coinType = Random.Range(0, coinSprites.Count);
        go.spriteRenderer.sprite = coinSprites[coinType];
        go.coin_type = (Coin.Coin_type)coinType;
        return go;
    }
}
