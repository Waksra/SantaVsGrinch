using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventAfterTime : MonoBehaviour
{
    [SerializeField] private bool onAwake = false;
    [SerializeField] private float delay = 1f;
    [SerializeField] private UnityEvent onTimeEvent;

    private void Awake()
    {
        if (onAwake)
            StartTimer();
    }
    
    public void StartTimer()
    {
        StartCoroutine(NotifyAfterTime());
    }

    private IEnumerator NotifyAfterTime()
    {
        yield return new WaitForSeconds(delay);
        onTimeEvent?.Invoke();
    }
}
