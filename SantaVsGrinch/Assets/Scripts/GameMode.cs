using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMode : MonoBehaviour
{
    private List<PlayerScore> playerScores = new List<PlayerScore>();
    [SerializeField] private int maxLives = 3;

    private void Start()
    {
        playerScores.Add(new PlayerScore(0, maxLives));
        playerScores.Add(new PlayerScore(1, maxLives));
    }

    private void CheckGameState()
    {
        foreach (var playerScore in playerScores) 
            if (playerScore.lives <= 0)
                EndMatch();
    }

    private void EndMatch()
    {
        Debug.Log("Game finished.");
    }

    private void AddPlayer(int playerIndex)
    {
        playerScores.Add(new PlayerScore(playerIndex, maxLives));
    }

    public void LoseLife(int playerIndex)
    {
        Debug.Log($"player {playerIndex} lost a life.");
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i].playerId == playerIndex)
            {
                PlayerScore playerScore = playerScores[i];
                playerScore.lives--;
                playerScores[i] = playerScore;
            }
        }

        CheckGameState();
    }
}

public struct PlayerScore
{
    public int playerId;
    public int lives;

    public PlayerScore(int playerId, int lives)
    {
        this.playerId = playerId;
        this.lives = lives;
    }
}