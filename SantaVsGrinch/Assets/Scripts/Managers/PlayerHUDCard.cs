using TMPro;
using UnityEngine;

public class PlayerHUDCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private GameObject[] lifeIcons = default;
    
    public void UpdateCard(float health, int remainingLives)
    {
        healthText.text = Mathf.CeilToInt(health).ToString();
        
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < remainingLives) 
                lifeIcons[i].SetActive(true);
            else
                lifeIcons[i].SetActive(false);
        }
    }
}
