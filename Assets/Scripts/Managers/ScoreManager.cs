using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IManager
{
    public ManagerStatus status { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public string PlayerName { get; private set; }
    public void Init()
    {
        Debug.Log("Score manager starting...");

        Score = 0;
        HighScore = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");


        status = ManagerStatus.Started;
    }


    private void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.ENEMY_IS_DEAD, OnScoreUpdate);
        Messenger<int>.AddListener(GameEvent.LEVEL_BEATEN, OnScoreUpdate);
        Messenger.AddListener(GameEvent.GAME_OVER, OnGameOver);
    }

    private void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.ENEMY_IS_DEAD, OnScoreUpdate);
        Messenger<int>.RemoveListener(GameEvent.LEVEL_BEATEN, OnScoreUpdate);
        Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
    }

    public void UpdateScore(int delta = 1)
    {
        Score += delta;
    }

    private void OnScoreUpdate(int score)
    {
        UpdateScore(score);
        Messenger.Broadcast(GameEvent.SCORE_UPDATED);
    }
    private void OnGameOver()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.SetString("Player", PlayerName);
        }
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }

    public void Reset()
    {
        Score = 0;
    }
}
