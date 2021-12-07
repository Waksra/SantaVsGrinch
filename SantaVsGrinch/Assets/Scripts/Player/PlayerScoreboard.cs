using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScoreboard : MonoBehaviour
{
    public void OnBackToMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.StartCharacterSelection();
        }
    }
}
