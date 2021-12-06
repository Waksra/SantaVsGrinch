using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventAfterTime : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    [SerializeField] private UnityEvent onTimeEvent;

    private void Awake()
    {
        StartCoroutine(NotifyAfterTime());
    }
    
    private IEnumerator NotifyAfterTime()
    {
        yield return new WaitForSeconds(delay);
        onTimeEvent?.Invoke();
    }
}
