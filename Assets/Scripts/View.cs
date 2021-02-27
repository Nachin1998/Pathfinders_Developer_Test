using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private Coin coinPrefab = null;
    [SerializeField] private List<Sprite> coinSprites = null;
    [SerializeField] private Vector2 coinOffset = Vector2.zero;
    [SerializeField] private Transform parent = null;

    [SerializeField] private LineRenderer lineRenderer = null;

    private int[] maxTypes;

    public Vector2 CoinOffset { get { return coinOffset; } }

    private List<GameObject> fallingCoins = null;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        fallingCoins = new List<GameObject>();
    }

    public Coin CreateCoin(int index, Vector3 pos, List<Coin.COIN_TYPE> typesAllowed)
    {
        Coin go = Instantiate(coinPrefab, pos, Quaternion.identity, parent);        

        go.gameObject.name = "Coin " + index;
        go.transform.position = pos;
        go.transform.parent = parent;

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

    public void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }

    public void AddLine(Vector3 newPos)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
    }

    public void EraseLine()
    {
        lineRenderer.positionCount--;
    }
}
