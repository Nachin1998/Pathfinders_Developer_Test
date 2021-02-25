using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI turnsLeftText;
    public TextMeshProUGUI canKeepPlaying;

    public Controller controller;
    void Awake()
    {
        controller.OnTurnUpdate += UpdateTurns;
        controller.OnScoreUpdate += UpdateScore;
    }

    void UpdateTurns(int turns)
    {
        turnsLeftText.text = "Turns left: " + turns;

        if (turns <= 0)
        {
            canKeepPlaying.gameObject.SetActive(true);
        }
        else
        {
            canKeepPlaying.gameObject.SetActive(false);
        }
    }

    void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}
