using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private GameManager gameManager;
    
    [SerializeField] private MainMenuButton[] buttons = default;
    [SerializeField] private GameObject menuPanel = default;
    [SerializeField] private GameObject creditsPanel = default;
    [SerializeField] private GameObject optionsPanel = default;
    [SerializeField] private MainMenuButton[] optionsButtons = default;
    [SerializeField] private Slider[] sliders = default;
    [SerializeField] private MainMenuButton[] creditsButtons = default;
    private int buttonIndex;

    [SerializeField] private float cooldown = 0.1f;
    private bool onCooldown;
    
    private void Awake()
    {
        gameManager = GameManager.instance;
        gameManager.JoinPlayersInMainMenu();
    }

    private void Start()
    {
        buttons[buttonIndex].Activate();
    }

    public void ChangeButtonIndex(int value, bool mouse)
    {
        if (!mouse && onCooldown)
            return;
        
        StartCoroutine(Cooldown());
        
        HandleOldSelection();

        if (optionsPanel.activeSelf)
        {
            buttonIndex += value;
            if (buttonIndex >= optionsButtons.Length)
                buttonIndex = 0;
            else if (buttonIndex < 0)
                buttonIndex = optionsButtons.Length - 1;
        } 
        else if (creditsPanel.activeSelf)
        {
            buttonIndex += value;
            if (buttonIndex >= creditsButtons.Length)
                buttonIndex = 0;
            else if (buttonIndex < 0)
                buttonIndex = creditsButtons.Length - 1;
        }
        else
        {
            buttonIndex += value;
            if (buttonIndex >= buttons.Length)
                buttonIndex = 0;
            else if (buttonIndex < 0)
                buttonIndex = buttons.Length - 1;
        }
        
        HandleNewSelection();
    }

    public void ChangeButtonIndexMouse(MainMenuButton button)
    {
        for (int i = 0; i < buttons.Length; i++)
            if (buttons[i].Equals(button))
                ChangeButtonIndex(i, true);
    }

    public void ChangeVolume(int value, bool mouse)
    {
        if (!mouse && onCooldown)
            return;
        
        StartCoroutine(Cooldown());

        if (buttonIndex < sliders.Length)
            sliders[buttonIndex].value += value * (sliders[buttonIndex].maxValue - sliders[buttonIndex].minValue) * 0.1f;
    }

    public void Confirm()
    {
        if (!optionsPanel.activeSelf && !creditsPanel.activeSelf)
        {
            switch (buttonIndex)
            {
                case 0:
                    StartLocalGame();
                    break;
                case 1:
                    ShowCredits();
                    break;
                case 2:
                    ShowOptions();
                    break;
                case 3:
                    ExitGame();
                    break;
            }
        }
        else if (optionsPanel.activeSelf)
        {
            switch (buttonIndex)
            {
                case 3:
                    BackToMenu();
                    break;
            }
        }
    }

    public void Cancel()
    {
        BackToMenu();
    }

    private void HandleNewSelection()
    {
        if (optionsPanel.activeSelf)
            optionsButtons[buttonIndex].Activate();
        else
            buttons[buttonIndex].Activate();
    }
    
    private void HandleOldSelection()
    {
        if (optionsPanel.activeSelf)
            optionsButtons[buttonIndex].Deactivate();
        else
            buttons[buttonIndex].Deactivate();
    }

    private void StartLocalGame()
    {
        gameManager.StartCharacterSelection();
    }

    private void ShowCredits()
    {
        creditsPanel.SetActive(true);
        menuPanel.SetActive(false);
        
        buttonIndex = 0;
        creditsButtons[buttonIndex].Activate();
    }

    private void ShowOptions()
    {
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
        
        buttonIndex = 0;
        optionsButtons[buttonIndex].Activate();
    }

    private void BackToMenu()
    {
        if (!creditsPanel.activeSelf && !optionsPanel.activeSelf) return;

        menuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        
        buttonIndex = 0;
        buttons[buttonIndex].Activate();
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSecondsRealtime(cooldown);
        onCooldown = false;
    }
}
