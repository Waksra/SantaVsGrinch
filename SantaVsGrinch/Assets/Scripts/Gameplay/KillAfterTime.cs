using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField] private bool onAwake = false;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private UnityEvent onDeathEvent;
    
    private void Awake()
    {
        if (onAwake)
            StartTimer();
    }

    public void StartTimer()
    {
        StartCoroutine(DieAfterTime());
    }
    
    private IEnumerator DieAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Die();
    }

    private void Die()
    {
        onDeathEvent.Invoke();
        Destroy(gameObject);
    }
}
