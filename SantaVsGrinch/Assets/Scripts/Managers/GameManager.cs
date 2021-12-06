using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private PlayerInputManager pim;
    
    private List<PlayerProfile> playerProfiles = new List<PlayerProfile>();
    [SerializeField] private GameObject charSelectorPrefab = default;
    [SerializeField] private GameObject[] characters = default;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        
        DontDestroyOnLoad(gameObject);
        pim = GetComponent<PlayerInputManager>();
    }

    public void StartMatch()
    {
        Debug.Log("Starting Match.");
        SceneManager.LoadScene(3);
    }
    
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                Debug.LogWarning("Trying to join player at buildIndex 0. Is this not the Main Menu?");
                break;
            case 1:
                FindObjectOfType<CharSelectionManager>().JoinPlayerInCharSelection(playerInput);
                break;
            case 2:
                Debug.LogWarning("Trying to join player at buildIndex 2. Is this not the Post-Match?");
                break;
            default:
                JoinPlayerInMatch(playerInput);
                break;
        }
    }

    private void JoinPlayerInMatch(PlayerInput playerInput)
    {
        GameObject.FindObjectOfType<CinemachineTargetGroup>().AddMember(playerInput.transform, 0.5f, 1f);
    }
    
    public void JoinPlayers()
    {
        foreach (PlayerProfile p in playerProfiles)
        {
            Debug.Log(p.characterId);
            pim.playerPrefab = characters[p.characterId];
            pim.JoinPlayer(p.id, p.id, null, p.devices);
        }
    }

    public void AddPlayer(PlayerProfile playerProfile)
    {
        playerProfiles.Add(playerProfile);
    }

    public void LockPlayerSelections(List<PlayerStateProfile> playerStateProfiles)
    {
        // Set characterId to setup for JoinPlayers when match starts
        foreach (PlayerProfile player in playerProfiles)
        {
            player.characterId = playerStateProfiles[player.id].teamIndex - 1;
        }
    }

    public void ResetForNewMatch()
    {
        playerProfiles.Clear();
        pim.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        pim.playerPrefab = charSelectorPrefab;
    }
}

public class PlayerProfile
{
    public int id { get; set; }
    public InputDevice[] devices { get; set; }
    public int characterId { get; set; }
}