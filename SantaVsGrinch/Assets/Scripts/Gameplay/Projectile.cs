using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody body;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float knockback = 1f;
    [SerializeField] private float dieAfterTime = 5f;
    [SerializeField] private GameObject onHitParticlesPrefab;
    [SerializeField] private bool ignoreInstigator = true;
    
    private int instigatorPlayerId;
    
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        StartCoroutine(DieAfterTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ignoreInstigator && other.GetComponent<PlayerController>().GetPlayerId() == instigatorPlayerId) return;
            
            other.GetComponent<Damageable>().TakeDamage(damage);
            Vector3 knockbackDir = new Vector3(body.velocity.x, 0f, body.velocity.z).normalized;
            other.GetComponent<Knockbackable>().Knockback(knockbackDir * knockback);
        }
        else
        {
            Die();
        }
    }

    public void Fire(Vector3 force)
    {
        body.AddForce(force, ForceMode.Impulse);
    }

    public void SetInstigator(int playerId)
    {
        instigatorPlayerId = playerId;
    }

    private IEnumerator DieAfterTime()
    {
        yield return new WaitForSeconds(dieAfterTime);
        Die();
    }

    private void Die()
    {
        if (onHitParticlesPrefab != null)
            Instantiate(onHitParticlesPrefab, transform.position, Quaternion.identity);
        else 
            Debug.LogWarning("OnHitParticles not selected.");
        
        Destroy(gameObject);
    }
}
