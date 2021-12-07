using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private PlayerInputManager pim;
    private GameMode gameMode;
    
    private List<PlayerProfile> playerProfiles = new List<PlayerProfile>();
    public List<PlayerProfile> GetPlayerProfiles() => playerProfiles;
    [SerializeField] private GameObject charSelectorPrefab = default;
    [SerializeField] private GameObject playerScoreboardPrefab = default;
    [SerializeField] private GameObject[] characters = default;
    [SerializeField] private string[] characterNames = default;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        
        DontDestroyOnLoad(gameObject);
        pim = GetComponent<PlayerInputManager>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(2);
    }

    public void StartMatch()
    {
        Debug.Log("Starting Match.");
        SceneManager.LoadScene("Scene_01");
        gameMode = FindObjectOfType<GameMode>();
    }
    
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                Debug.LogWarning("Trying to join player at buildIndex 0. Aren't we in the Preload scene?");
                break;
            case 1:
                Debug.LogWarning("Trying to join player at buildIndex 1. Is this not the Main Menu?");
                break;
            case 2:
                FindObjectOfType<CharSelectionManager>().JoinPlayerInCharSelection(playerInput);
                break;
            case 3:
                Debug.Log("Joined players at Scoreboard.");
                break;
            default:
                JoinPlayerInMatch(playerInput);
                break;
        }
    }
    
    public void JoinPlayersInScoreboard()
    {
        foreach (PlayerProfile p in playerProfiles)
        {
            pim.playerPrefab = playerScoreboardPrefab;
            pim.JoinPlayer(p.id, p.id, null, p.devices);
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

    public void StartCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    public void StartScoreboard()
    {
        SceneManager.LoadScene("Scoreboard");
    }

    public string GetCharacterNameById(int id)
    {
        return characterNames[id];
    }
}

public class PlayerProfile
{
    public int id { get; set; }
    public InputDevice[] devices { get; set; }
    public int characterId { get; set; }
}