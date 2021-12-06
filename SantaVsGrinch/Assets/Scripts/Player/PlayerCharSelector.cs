using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharSelector : MonoBehaviour
{
    private AudioSource audioSource;

    #region AudioClips

    // [SerializeField] private AudioClip buttonConfirmClip = default;
    // [SerializeField] private AudioClip buttonCancelClip = default;
    // [SerializeField] private AudioClip moveButtonClip = default;

    #endregion

    #region Delegates

    public delegate void ConfirmDelegate(PlayerCharSelector playerCharSelector);
    public event ConfirmDelegate ConfirmEvent;
    
    public delegate void CancelDelegate(PlayerCharSelector playerCharSelector);
    public event CancelDelegate CancelEvent;

    public delegate void MovedLeftDelegate(PlayerCharSelector playerCharSelector);
    public event MovedLeftDelegate MovedLeftEvent;

    public delegate void MovedRightDelegate(PlayerCharSelector playerCharSelector);
    public event MovedRightDelegate MovedRightEvent;

    #endregion
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // audioSource.PlayOneShot(buttonConfirmClip);
        ConfirmEvent.Invoke(this);
    }
    
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // audioSource.PlayOneShot(buttonCancelClip);
        CancelEvent.Invoke(this);
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // audioSource.PlayOneShot(moveButtonClip);
        MovedLeftEvent.Invoke(this);
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // audioSource.PlayOneShot(moveButtonClip);
        MovedRightEvent.Invoke(this);
    }
}
