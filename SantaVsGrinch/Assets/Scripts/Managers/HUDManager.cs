using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private GameMode gameMode;
    private Damageable[] playerDamageables = new Damageable[2];
    private List<PlayerScore> playerScores;
    
    [SerializeField] private TextMeshProUGUI[] healthTexts = default;
    [SerializeField] private GameObject[] lifeIcons = default;

    private void Start()
    {
        gameMode = GetComponent<GameMode>();
        PlayerInput[] playerInputs = gameMode.GetPlayerInputs();
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerDamageables[i] = playerInputs[i].GetComponent<Damageable>();
        }

        playerScores = gameMode.GetPlayerScores();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < playerDamageables.Length; i++)
        { 
            healthTexts[i].text = playerDamageables[i].Health.ToString();
            
            //TODO: Life icons, maybe a player hud script to handle each player's icons, or a list player1icons and player2icons
            // for (int j = 0; j < playerScores.Count; j++)
            // {
            //     playerScores
            //     lifeIcons[i].SetActive(false);
            // }
        }
    }
}
