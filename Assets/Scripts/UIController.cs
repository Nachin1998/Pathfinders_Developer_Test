using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI turnsLeftText;
    public TextMeshProUGUI canKeepPlaying;

    void Start()
    {
        Controller.OnTurnChanged += UpdateTurns;
        Controller.OnTurnChanged += UpdateCanKeepPlaying;
    }

    void UpdateTurns(int turns)
    {
        turnsLeftText.text = "Turns left: " + turns;
    }

    void UpdateCanKeepPlaying(int turns)
    {
        if(turns <= 0)
        {
            canKeepPlaying.gameObject.SetActive(true);
        }
        else
        {
            canKeepPlaying.gameObject.SetActive(false);
        }
    }
}
