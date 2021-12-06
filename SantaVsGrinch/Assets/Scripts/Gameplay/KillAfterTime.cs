using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private UnityEvent onDeathEvent;
    
    private void Awake()
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
        onDeathEvent?.Invoke();
        Destroy(gameObject);
    }
}
