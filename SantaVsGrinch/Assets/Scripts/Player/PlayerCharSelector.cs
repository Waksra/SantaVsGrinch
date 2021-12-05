using UnityEngine;

public class PlayerCharSelector : MonoBehaviour
{
    private AudioSource audioSource;

    #region AudioClips

    [SerializeField] private AudioClip buttonConfirmClip = default;
    [SerializeField] private AudioClip buttonCancelClip = default;
    [SerializeField] private AudioClip moveButtonClip = default;

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

    private void OnConfirm()
    {
        audioSource.PlayOneShot(buttonConfirmClip);
        ConfirmEvent.Invoke(this);
    }
    
    private void OnCancel()
    {
        audioSource.PlayOneShot(buttonCancelClip);
        CancelEvent.Invoke(this);
    }

    private void OnMoveLeft()
    {
        audioSource.PlayOneShot(moveButtonClip);
        MovedLeftEvent.Invoke(this);
    }

    private void OnMoveRight()
    {
        audioSource.PlayOneShot(moveButtonClip);
        MovedRightEvent.Invoke(this);
    }
}
