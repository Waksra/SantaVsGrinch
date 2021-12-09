using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private GameObject[] lifeIcons = default;
    [SerializeField] private Image[] weaponSlotIcons = default;
    
    public void UpdateCard(float health, int remainingLives)
    {
        // Health text
        healthText.text = Mathf.CeilToInt(health).ToString();
        
        // Health icons
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < remainingLives) 
                lifeIcons[i].SetActive(true);
            else
                lifeIcons[i].SetActive(false);
        }
    }

    public void UpdateWeapon(int slotIndex, Sprite icon)
    {
        weaponSlotIcons[slotIndex].sprite = icon;
    }
}
