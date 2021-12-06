using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMode : MonoBehaviour
{
    private List<PlayerScore> playerScores = new List<PlayerScore>();
    [SerializeField] private int maxLives = 3;

    [SerializeField] private Transform[] spawnPoints = default;

    private PlayerInput[] playerInputs = new PlayerInput[2];
    
    private void Start()
    {
        playerScores.Add(new PlayerScore(0, maxLives));
        playerScores.Add(new PlayerScore(1, maxLives));
        
        GameObject.FindGameObjectWithTag("GameController").GetComponent<CharSelectionManager>().JoinPlayers();
        PlayerInput[] foundInputs = GameObject.FindObjectsOfType<PlayerInput>();
        foreach (var foundInput in foundInputs) playerInputs[foundInput.playerIndex] = foundInput;
        foreach (var playerInput in playerInputs) AddPlayer(playerInput.playerIndex);

        SpawnPlayers();
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
        StartCoroutine(RespawnAfterTime(playerIndex));
    }

    private void SpawnPlayers()
    {
        foreach (var playerInput in playerInputs)
        {
            int playerIndex = playerInput.playerIndex;
            playerInputs[playerIndex].GetComponent<Rigidbody>().velocity = Vector3.zero;
            playerInputs[playerIndex].GetComponent<Rigidbody>().position = spawnPoints[playerIndex].position;
            playerInputs[playerIndex].GetComponent<Rigidbody>().rotation = spawnPoints[playerIndex].rotation;
            playerInputs[playerIndex].GetComponent<Damageable>().Respawn();
        }
    }

    private void RespawnPlayer(int playerIndex)
    {
        float longestDistFound = 0f;
        Transform furthestSpawnPoint = transform;
        int otherPlayerIndex = playerIndex == 0 ? 1 : 0;
        foreach (Transform spawnPoint in spawnPoints)
        {
            float dist = Vector3.Distance(playerInputs[otherPlayerIndex].transform.position, spawnPoint.position);
            if (longestDistFound < dist)
            {
                longestDistFound = dist;
                furthestSpawnPoint = spawnPoint;
            }
        }

        playerInputs[playerIndex].GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerInputs[playerIndex].GetComponent<Rigidbody>().position = furthestSpawnPoint.position;
        playerInputs[playerIndex].GetComponent<Rigidbody>().rotation = furthestSpawnPoint.rotation;
        playerInputs[playerIndex].GetComponent<Damageable>().Respawn();
    }

    private IEnumerator RespawnAfterTime(int playerIndex)
    {
        yield return new WaitForSecondsRealtime(2f);
        RespawnPlayer(playerIndex);
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