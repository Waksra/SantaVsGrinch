using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMainMenu : MonoBehaviour
{
    private MainMenuManager mainMenuManager;

    private bool[] buttonDowns = new bool[4];
    
    private void Awake()
    {
        mainMenuManager = FindObjectOfType<MainMenuManager>();
    }

    private void Update()
    {
        if (buttonDowns[0]) mainMenuManager.ChangeButtonIndex(-1, false);
        if (buttonDowns[1]) mainMenuManager.ChangeButtonIndex(1, false);
        if (buttonDowns[2]) mainMenuManager.ChangeVolume(-1, false);
        if (buttonDowns[3]) mainMenuManager.ChangeVolume(1, false);
    }

    private void ChangeButtonIndex(InputAction.CallbackContext context, int index)
    {
        if (context.performed)
            buttonDowns[index] = true;
        else if (context.canceled)
            buttonDowns[index] = false;
    }

    public void OnMoveUp(InputAction.CallbackContext context)
    {
        ChangeButtonIndex(context, 0);
    }
    
    public void OnMoveDown(InputAction.CallbackContext context)
    {
        ChangeButtonIndex(context, 1);
    }
    
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        ChangeButtonIndex(context, 2);
    }
    
    public void OnMoveRight(InputAction.CallbackContext context)
    {
        ChangeButtonIndex(context, 3);
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        mainMenuManager.Confirm();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        mainMenuManager.Cancel();
    }
}
