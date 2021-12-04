using System.Collections;
using UnityEngine;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField] private float dieAfterTime = 5f;
    
    private void Awake()
    {
        StartCoroutine(DieAfterTime());
    }
    
    private IEnumerator DieAfterTime()
    {
        yield return new WaitForSeconds(dieAfterTime);
        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
