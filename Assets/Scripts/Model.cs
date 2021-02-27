using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    [SerializeField] private int maxSizeX = 0;
    [SerializeField] private int maxSizeY = 0;

    int[,] grid;

    public static float RoundToNearestHalf(float a)
    {
        return a = Mathf.Round(a * 2f) * 0.5f;
    }

    public int[,] GetGrid(ref int maxX, ref int maxY)
    {
        maxX = maxSizeX;
        maxY = maxSizeY;

        return grid;
    }

    public IEnumerator DropCoinDown(GameObject go)
    {
        if (go.transform.position.y <= 0)
        {
            go.transform.position = new Vector3(go.transform.position.x, 0);
            yield break;
        }

        RaycastHit2D hit2D = Physics2D.Raycast(go.transform.position - new Vector3(0, 0.6f), new Vector2(0, -1.5f), 0.5f);
        Debug.DrawRay(go.transform.position - new Vector3(0, 0.6f), new Vector3(0, -1.5f));

        if (!hit2D)
        {
            transform.position += Vector3.down * 2 * Time.deltaTime;
        }
        yield return null;
    }

    public bool CanMoveToPosition(GameObject coin1, GameObject coin2, Vector2 offset)
    {
        //Checks diagonal chaining
        if((coin1.transform.position.x >= coin2.transform.position.x + offset.x || 
            coin1.transform.position.x <= coin2.transform.position.x - offset.x) &&
           (coin1.transform.position.y >= coin2.transform.position.y + offset.y ||
            coin1.transform.position.y <= coin2.transform.position.y - offset.y))
        {
            return false;
        }

        //Checks further away chaining
        if ((coin1.transform.position.x > coin2.transform.position.x + offset.x ||
             coin1.transform.position.x < coin2.transform.position.x - offset.x))
        {
            return false;
        }
        if ((coin1.transform.position.y > coin2.transform.position.y + offset.y ||
             coin1.transform.position.y < coin2.transform.position.y - offset.y))
        {
            return false;
        }

        return true;
    }
}
