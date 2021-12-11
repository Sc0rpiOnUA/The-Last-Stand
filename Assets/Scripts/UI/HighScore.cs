using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HighScore : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;
    private int score;

    private void OnEnable()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (PlayerPrefs.HasKey("Score")) score = PlayerPrefs.GetInt("Score");
        else score = 0;

        if (score == 0) scoreText.text = "NONE";
        else scoreText.text = score.ToString();
    }

    public void ResetScore()
    {
        if (PlayerPrefs.HasKey("Score")) PlayerPrefs.DeleteKey("Score");
        UpdateScore();
    }
}
