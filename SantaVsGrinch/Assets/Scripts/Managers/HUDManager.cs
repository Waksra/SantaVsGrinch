using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUDManager : MonoBehaviour
{
    private GameMode gameMode;
    private Damageable[] playerDamageables = new Damageable[2];
    private List<PlayerScore> playerScores;
    
    [SerializeField] private PlayerHUDCard[] playerHUDCards = default;
    
    private void Start()
    {
        gameMode = FindObjectOfType<GameMode>();
        
        PlayerInput[] playerInputs = gameMode.GetPlayerInputs();
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerDamageables[i] = playerInputs[i].GetComponent<Damageable>();
        }

        playerScores = gameMode.GetPlayerScores();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < playerHUDCards.Length; i++)
        {
            int remainingLives = gameMode.MaxLives - playerScores[i].deaths;
            playerHUDCards[i].UpdateCard(playerDamageables[i].Health, remainingLives);
            
            playerHUDCards[i].SetIcon(GameManager.instance.GetCharacterIconByPlayerIndex(i));
        }
    }
    
    public void UpdateWeapon(int playerIndex, WeaponInfo info)
    {
        playerHUDCards[playerIndex].UpdateWeapon(info.SlotIndex, info.HUDIcon);
    }

    public void ClearWeapon(int playerIndex, int slot)
    {
        playerHUDCards[playerIndex].ClearWeapon(slot);
    }
}
