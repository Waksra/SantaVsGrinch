using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharSelectionManager : MonoBehaviour
{
    private PlayerInputManager pim;
    private List<PlayerProfile> playerProfiles = default;

    private List<PlayerCharSelector> playerCharSelectors = new List<PlayerCharSelector>();
    private List<PlayerCharSelectionProfile> playerCharSelectionProfiles = new List<PlayerCharSelectionProfile>();
    
    [SerializeField] private int maxPlayerAmount = 2;
    [SerializeField] private Transform canvas = default;
    [SerializeField] private GameObject[] deviceIcons = default;
    [SerializeField] private Sprite[] deviceSprite = default;

    [SerializeField] private GameObject[] characters = default;
    
    private int deviceIconsIndex = 0;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        pim = PlayerInputManager.instance;

        playerProfiles.Clear();
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                Debug.LogWarning("Trying to join player at buildIndex 0. Is this not the Main Menu?");
                break;
            case 1:
                JoinPlayerInCharSelection(playerInput);
                break;
            case 2:
                Debug.LogWarning("Trying to join player at buildIndex 2. Is this not the Post-Match?");
                break;
            default:
                JoinPlayerInMatch(playerInput);
                break;
        }
    }

    private void JoinPlayerInCharSelection(PlayerInput playerInput)
    {
        if (pim.playerCount > maxPlayerAmount)
            pim.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

        PlayerProfile playerProfile = new PlayerProfile()
        {
            id = playerInput.playerIndex,
            devices = playerInput.devices.ToArray(),
        };
        
        PlayerCharSelectionProfile playerCharSelectionProfiles = new PlayerCharSelectionProfile()
        {
            id = playerInput.playerIndex,
        };
        
        playerProfiles.Add(playerProfile);
        playerCharSelectors.Add(playerInput.GetComponent<PlayerCharSelector>());
    }

    private void JoinPlayerInMatch(PlayerInput playerInput)
    {
        Debug.LogWarning("Trying to join player in match but no logic is executed.");
    }
    
    private void SetDeviceIcon()
    {
        // deviceIcons[deviceIconsIndex].GetComponent<Image>().sprite = deviceSprite[i];
    }
    
    private void AddSubscriberToAll(int i)
    {
        playerCharSelectors[i].MovedLeftEvent += OnMovedLeft;
        playerCharSelectors[i].MovedRightEvent += OnMovedRight;
        playerCharSelectors[i].ConfirmEvent += OnConfirm;
        playerCharSelectors[i].CancelEvent += OnCancel;
    }

    private void OnReadyCheck()
    {
        bool allReady = true;
        foreach (PlayerCharSelectionProfile profile in playerCharSelectionProfiles)
        {
            if (!profile.isReady)
            {
                allReady = false;
            }
        }

        if (allReady)
        {
            StartMatch();
        }
    }

    private void StartMatch()
    {
        Debug.Log("Starting Match.");
    }

    private void OnConfirm(PlayerCharSelector playerCharSelector)
    {
        int index = playerCharSelectors.IndexOf(playerCharSelector);
        playerCharSelectionProfiles[index].isReady = true;

        OnReadyCheck();
    }
    
    private void OnCancel(PlayerCharSelector playerCharSelector)
    {
        int index = playerCharSelectors.IndexOf(playerCharSelector);
        playerCharSelectionProfiles[index].isReady = false;
    }
    
    private void OnMovedLeft(PlayerCharSelector playerCharSelector)
    {
        int index = playerCharSelectors.IndexOf(playerCharSelector);
        int teamIndex = playerCharSelectionProfiles[index].teamIndex;

        if (teamIndex == (int) TeamSelectionIndex.Neutral)
            teamIndex = (int) TeamSelectionIndex.Team1;
        else if (teamIndex == (int)TeamSelectionIndex.Team2) 
            teamIndex = (int) TeamSelectionIndex.Neutral;
    }

    private void OnMovedRight(PlayerCharSelector playerCharSelector)
    {
        int index = playerCharSelectors.IndexOf(playerCharSelector);
        int teamIndex = playerCharSelectionProfiles[index].teamIndex;

        if (teamIndex == (int) TeamSelectionIndex.Neutral)
            teamIndex = (int) TeamSelectionIndex.Team2;
        else if (teamIndex == (int)TeamSelectionIndex.Team1) 
            teamIndex = (int) TeamSelectionIndex.Neutral;
    }

    // When match starts (once the scene is loaded and players are ready to play)
    public void JoinPlayers()
    {
        foreach (PlayerProfile p in playerProfiles)
        {
            pim.playerPrefab = characters[p.characterId];
            pim.JoinPlayer(p.id, p.id, null, p.devices);
        }
    }

    private enum TeamSelectionIndex
    {
        Neutral = 0,
        Team1 = 1,
        Team2 = 2,
    }
}

public class PlayerProfile
{
    public int id { get; set; }
    public InputDevice[] devices { get; set; }
    public int characterId { get; set; }
}

public class PlayerCharSelectionProfile
{
    public int id { get; set; }
    public int teamIndex { get; set; }
    public bool isReady { get; set; }
}
