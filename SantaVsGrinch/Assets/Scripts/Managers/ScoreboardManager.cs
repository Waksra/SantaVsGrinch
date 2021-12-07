using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardManager : MonoBehaviour
{
    private List<PlayerProfile> playerProfiles;
    private List<PlayerScore> playerScores;
    
    [SerializeField] private TextMeshProUGUI[] characterNameTexts = default;
    [SerializeField] private TextMeshProUGUI[] killsTexts = default;
    [SerializeField] private TextMeshProUGUI[] deathsTexts = default;
    [SerializeField] private TextMeshProUGUI[] damageDealtTexts = default;

    private void Awake()
    {
        FindObjectOfType<GameManager>().JoinPlayersInScoreboard();
    }

    private void Start()
    {
        playerProfiles = FindObjectOfType<GameManager>().GetPlayerProfiles();
        playerScores = FindObjectOfType<GameMode>().GetPlayerScores();
        SetTexts();
    }

    private void SetTexts()
    {
        for (int i = 0; i < playerProfiles.Count; i++)
        {
            characterNameTexts[i].text = GameManager.instance.GetCharacterNameById(playerProfiles[i].characterId);
            killsTexts[i].text = playerScores[i].kills.ToString();
            deathsTexts[i].text = playerScores[i].deaths.ToString();
            damageDealtTexts[i].text = playerScores[i].damageDealt.ToString();
        }
    }
}
