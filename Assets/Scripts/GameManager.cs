using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private enum Token_type
    {
        WATER,
        EARTH,
        FIRE,
        AIR
    }
    [SerializeField] private Token_type token_Type; 

    [SerializeField] private List<Sprite> tokenSprites = null;
    [SerializeField] private Vector2 tokenOffset = Vector2.zero;
    [SerializeField] private Vector2 boxColliderSize = Vector2.zero;
    [SerializeField] private Transform parent = null;
    [SerializeField] private int maxSizeX = 0;
    [SerializeField] private int maxSizeY = 0;    

    private int[,] grid = null;
    private GameObject[,] gridObjects = null;

    void Start()
    {
        grid = new int[maxSizeX, maxSizeY];
        gridObjects = new GameObject[maxSizeX, maxSizeY];
        int index = 0;
        
        for (int i = 0; i < maxSizeX; i++)
        {
            for (int j = 0; j < maxSizeY; j++)
            {
                gridObjects[i, j] = CreateCoin(index, new Vector3(tokenOffset.x * i, tokenOffset.y * j, 0));
                index++;
            }
        }
    }

    GameObject CreateCoin(int index, Vector3 pos)
    {
        GameObject go = new GameObject("Coin " + index);
        go.transform.parent = parent;
        go.transform.position = pos;

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tokenSprites[Random.Range(0, tokenSprites.Count)];

        return go;
    }
}
