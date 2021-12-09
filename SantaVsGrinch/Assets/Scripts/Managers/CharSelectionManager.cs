using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharSelectionManager : MonoBehaviour
{
    private PlayerInputManager pim;
    private GameManager gameManager;
    
    private List<PlayerStateProfile> playerStateProfiles = new List<PlayerStateProfile>(); // Player device and character selection data
    private List<PlayerCharSelector> playerCharSelectors = new List<PlayerCharSelector>(); // Player input class in Character Selection Scene
    
    [SerializeField] private int maxPlayerAmount = 2;
    [SerializeField] private GameObject[] deviceIcons = default;
    [SerializeField] private Sprite[] deviceSprite = default;
    [SerializeField] private GameObject[] helpTexts = default;

    [SerializeField] private float deviceIconOffset = 100f;

    private void Start()
    {
        pim = PlayerInputManager.instance;
        gameManager = FindObjectOfType<GameManager>();
        
        gameManager.ResetForNewMatch();
    }

    public void JoinPlayerInCharSelection(PlayerInput playerInput)
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
        
        gameManager.AddPlayer(playerProfile);
        playerCharSelectors.Add(playerInput.GetComponent<PlayerCharSelector>());
        playerStateProfiles.Add(playerStateProfile);

        AddSubscriberToAll(playerInput.playerIndex);

        int deviceIndex = -1;
        string controlScheme = playerInput.currentControlScheme;
        if (controlScheme == "KB/M")
            deviceIndex = 0;
        else if (controlScheme == "Gamepad")
            deviceIndex = 1;
        SetDeviceIcon(playerInput.playerIndex, deviceIndex);
    }
    
    private void SetDeviceIcon(int playerIndex, int deviceIndex)
    {
        deviceIcons[playerIndex].GetComponent<Image>().enabled = true;
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

        gameManager.LockPlayerSelections(playerStateProfiles);

        StartMatch();
    }

    private void StartMatch()
    {
        for (int i = playerCharSelectors.Count - 1; i >= 0; i--)
        {
            Destroy(playerCharSelectors[i].gameObject);
        }
        gameManager.StartMatch();
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
        if (!playerStateProfiles[index].isReady)
            playerStateProfiles[index].isReady = false;
        else
            gameManager.StartMenu();
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
    
    private enum TeamSelectionIndex
    {
        Neutral = 0,
        Team1 = 1,
        Team2 = 2,
    }
}

public class PlayerStateProfile
{
    public int id { get; set; }
    public int teamIndex { get; set; }
    public bool isReady { get; set; }
}
