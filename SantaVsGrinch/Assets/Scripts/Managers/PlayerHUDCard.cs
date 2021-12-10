using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private GameObject[] lifeIcons = default;
    [SerializeField] private Image[] weaponSlotIcons = default;

    private void Start()
    {
        foreach (var weaponSlotIcon in weaponSlotIcons)
        {
            if(weaponSlotIcon.sprite == null)
                weaponSlotIcon.enabled = false;
        }
    }

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
        weaponSlotIcons[slotIndex - 1].enabled = true;
        weaponSlotIcons[slotIndex - 1].sprite = icon;
    }

    public void ClearWeapon(int slotIndex)
    {
        weaponSlotIcons[slotIndex - 1].enabled = false;
    }
}
