using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    private GameManager gameManager;
    private List<PlayerProfile> playerProfiles;
    private List<PlayerScore> playerScores;
    
    [SerializeField] private Sprite[] characterIcons = default;
    [SerializeField] private Image[] characterImages = default;
    [SerializeField] private TextMeshProUGUI[] characterNameTexts = default;
    [SerializeField] private TextMeshProUGUI[] killsTexts = default;
    [SerializeField] private TextMeshProUGUI[] deathsTexts = default;
    [SerializeField] private TextMeshProUGUI[] damageDealtTexts = default;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.JoinPlayersInScoreboard();
    }

    private void Start()
    {
        playerProfiles = gameManager.GetPlayerProfiles();
        playerScores = gameManager.GetPlayerScores();
        SetTexts();
    }

    private void SetTexts()
    {
        for (int i = 0; i < playerProfiles.Count; i++)
        {
            characterImages[i].sprite = characterIcons[playerProfiles[i].characterId];
            characterNameTexts[i].text = GameManager.instance.GetCharacterNameById(playerProfiles[i].characterId);
            killsTexts[i].text = playerScores[i].kills.ToString();
            deathsTexts[i].text = playerScores[i].deaths.ToString();
            damageDealtTexts[i].text = playerScores[i].damageDealt.ToString();
        }
    }
}
