using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    //*Cricket noises*

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
