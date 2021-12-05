using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharSelectionManager : MonoBehaviour
{
    private PlayerInputManager pim;
    private List<PlayerProfile> playerProfiles = new List<PlayerProfile>();

    private List<PlayerCharSelector> playerCharSelectors = new List<PlayerCharSelector>(); // Player input class in Character Selection Scene
    private List<PlayerStateProfile> playerStateProfiles = new List<PlayerStateProfile>(); // Player device and character selection data
    
    [SerializeField] private int maxPlayerAmount = 2;
    [SerializeField] private Transform canvas = default;
    [SerializeField] private GameObject[] deviceIcons = default;
    [SerializeField] private Sprite[] deviceSprite = default;
    [SerializeField] private GameObject[] helpTexts = default;

    [SerializeField] private GameObject[] characters = default;
    [SerializeField] private float deviceIconOffset = 100f;

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
        if (pim.playerCount >= maxPlayerAmount)
            pim.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

        PlayerProfile playerProfile = new PlayerProfile()
        {
            id = playerInput.playerIndex,
            devices = playerInput.devices.ToArray(),
        };
        
        PlayerStateProfile playerStateProfile = new PlayerStateProfile()
        {
            id = playerInput.playerIndex,
        };
        
        playerProfiles.Add(playerProfile);
        playerCharSelectors.Add(playerInput.GetComponent<PlayerCharSelector>());
        playerStateProfiles.Add(playerStateProfile);

        AddSubscriberToAll(playerInput.playerIndex);

        int deviceIndex = 0;
        string deviceClass = playerInput.devices[0].device.description.deviceClass;
        if (deviceClass == "Keyboard" || deviceClass == "Mouse")
            deviceIndex = 1;
        SetDeviceIcon(playerInput.playerIndex, deviceIndex);
    }

    private void JoinPlayerInMatch(PlayerInput playerInput)
    {
        Debug.LogWarning("Trying to join player in match but no logic is executed.");
    }
    
    private void SetDeviceIcon(int playerIndex, int deviceIndex)
    {
        deviceIcons[playerIndex].GetComponent<Image>().sprite = deviceSprite[deviceIndex];
        helpTexts[playerIndex].SetActive(false);
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
        if (playerStateProfiles.Count <= 1) return;
        
        int[] teamSelected = new int[3];
        foreach (PlayerStateProfile player in playerStateProfiles)
        {
            if (!player.isReady)
            {
                Debug.Log("ReadyCheck failed, a player is not ready.");
                return;
            }

            teamSelected[player.teamIndex]++;
        }

        if (teamSelected[0] > 0 || teamSelected[1] > 1 || teamSelected[2] > 1){
            Debug.Log("ReadyCheck failed, players on the same team or teams not selected.");
            return;
        }

        // Set characterId to setup for JoinPlayers when match starts
        foreach (PlayerProfile player in playerProfiles)
        {
            player.characterId = playerStateProfiles[player.id].teamIndex - 1;
        }

        StartMatch();
    }

    private void StartMatch()
    {
        for (int i = playerCharSelectors.Count - 1; i >= 0; i--)
        {
            Destroy(playerCharSelectors[i].gameObject);
        }
        Debug.Log("Starting Match.");
        SceneManager.LoadScene(3);
    }

    private void OnConfirm(PlayerCharSelector playerCharSelector)
    {
        int index = playerCharSelectors.IndexOf(playerCharSelector);
        playerStateProfiles[index].isReady = true;

        OnReadyCheck();
    }
    
    private void OnCancel(PlayerCharSelector playerCharSelector)
    {
        int index = playerCharSelectors.IndexOf(playerCharSelector);
        playerStateProfiles[index].isReady = false;
    }
    
    private void OnMovedLeft(PlayerCharSelector playerCharSelector)
    {
        int playerIndex = playerCharSelectors.IndexOf(playerCharSelector);
        int teamIndex = playerStateProfiles[playerIndex].teamIndex;

        if (teamIndex == (int) TeamSelectionIndex.Neutral)
            teamIndex = (int) TeamSelectionIndex.Team1;
        else if (teamIndex == (int) TeamSelectionIndex.Team2)
            teamIndex = (int) TeamSelectionIndex.Neutral;
        else 
            return;
        
        playerStateProfiles[playerIndex].teamIndex = teamIndex;
        deviceIcons[playerIndex].transform.localPosition += deviceIconOffset * Vector3.left;
    }

    private void OnMovedRight(PlayerCharSelector playerCharSelector)
    {
        int playerIndex = playerCharSelectors.IndexOf(playerCharSelector);
        int teamIndex = playerStateProfiles[playerIndex].teamIndex;

        if (teamIndex == (int) TeamSelectionIndex.Neutral)
            teamIndex = (int) TeamSelectionIndex.Team2;
        else if (teamIndex == (int) TeamSelectionIndex.Team1)
            teamIndex = (int) TeamSelectionIndex.Neutral;
        else
            return;
        
        playerStateProfiles[playerIndex].teamIndex = teamIndex;
        deviceIcons[playerIndex].transform.localPosition += deviceIconOffset * Vector3.right;
    }

    // When match starts (once the scene is loaded and players are ready to play)
    public void JoinPlayers()
    {
        foreach (PlayerProfile p in playerProfiles)
        {
            Debug.Log("CharacterId: " + p.characterId);
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

public class PlayerStateProfile
{
    public int id { get; set; }
    public int teamIndex { get; set; }
    public bool isReady { get; set; }
}
