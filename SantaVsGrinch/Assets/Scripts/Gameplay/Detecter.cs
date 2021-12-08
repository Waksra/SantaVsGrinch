using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Detecter : MonoBehaviour
{
    [SerializeField] private bool isLever = false;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private UnityEvent onTriggeredEvent = default;
    
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask layerMask = default;

    private bool onCooldown;
    
    private void FixedUpdate()
    {
        if (onCooldown) return;
        
         Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (colliders.Length <= 0) return;
        
        foreach (Collider col in colliders)
        {
            if (cooldown > 0f)
            {
                onCooldown = true;
                StartCoroutine(Cooldown());
                onTriggeredEvent?.Invoke();
                return;
            }
            onTriggeredEvent?.Invoke();
        }
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(cooldown);
        onCooldown = false;
    }
}
