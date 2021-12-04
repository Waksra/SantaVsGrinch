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
        playerScores.Add(new PlayerScore(0));
        playerScores.Add(new PlayerScore(1));
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
        playerScores.Add(new PlayerScore(playerIndex));
    }

    public void LoseLife(int playerIndex)
    {
        PlayerScore playerScore = FindPlayerScoreByIndex(playerIndex);
        playerScore.lives--;
        
        CheckGameState();
    }

    private PlayerScore FindPlayerScoreByIndex(int playerIndex)
    {
        foreach (var playerScore in playerScores)
        {
            if (playerIndex == playerScore.playerId)
                return playerScore;
        }
        return default;
    }
}

public struct PlayerScore
{
    public int playerId;
    public int lives;

    public PlayerScore(int playerId)
    {
        this.playerId = playerId;
        lives = default;
    }
}