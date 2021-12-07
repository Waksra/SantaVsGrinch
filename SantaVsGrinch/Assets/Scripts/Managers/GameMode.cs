using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMode : MonoBehaviour
{
    public static GameMode instance;
    private List<PlayerScore> playerScores = new List<PlayerScore>();
    [SerializeField] private int maxLives = 3;
    public int MaxLives => maxLives;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float matchEndDelay = 1f;

    [SerializeField] private Transform[] spawnPoints = default;

    private PlayerInput[] playerInputs = new PlayerInput[2];
    public PlayerInput[] GetPlayerInputs() => playerInputs;
    public List<PlayerScore> GetPlayerScores() => playerScores;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        else
            instance = this;
        
        DontDestroyOnLoad(this);
        
        FindObjectOfType<GameManager>().JoinPlayers();
        PlayerInput[] foundInputs = GameObject.FindObjectsOfType<PlayerInput>();
        foreach (var foundInput in foundInputs) playerInputs[foundInput.playerIndex] = foundInput;
        foreach (var playerInput in playerInputs) AddPlayer(playerInput.playerIndex);
    }

    private void Start()
    {
        SpawnPlayers();
    }

    private void CheckGameState()
    {
        // Maximum game length (time) would be added here, or any other criteria.
        foreach (var playerScore in playerScores) 
            if (playerScore.deaths >= maxLives)
                StartCoroutine(EndMatchAfterTime());
    }

    private void EndMatch()
    {
        StopAllCoroutines();
        
        Debug.Log("Game finished.");
        GameManager.instance.StartScoreboard();
    }

    private void AddPlayer(int playerIndex)
    {
        playerScores.Add(new PlayerScore(playerIndex));
    }

    public void AddDeath(int playerIndex)
    {
        Debug.Log($"player {playerIndex} lost a life.");
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i].playerId == playerIndex)
            {
                PlayerScore playerScore = playerScores[i];
                playerScore.deaths++;
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
        yield return new WaitForSecondsRealtime(respawnDelay);
        RespawnPlayer(playerIndex);
    }
    
    private IEnumerator EndMatchAfterTime()
    {
        yield return new WaitForSecondsRealtime(matchEndDelay);
        EndMatch();
    }
}

public struct PlayerScore
{
    public int playerId;
    public int kills;
    public int deaths;
    // public int suicides;
    public float damageDealt;
    // public float damageTaken;
    // public int dashes;
    // public float accuracy;
    
    public PlayerScore(int playerId)
    {
        this.playerId = playerId;
        kills = 0;
        deaths = 0;
        damageDealt = 0;
    }
}