using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField, Range(0, 50)] private float initialVelocity = 20f;
        [SerializeField, Range(0, 5)] private float deathDelay = 0f;
        [SerializeField] private bool ignoreInstigator = true;
        [SerializeField, Range(-1, 60)] private float lifeTime = -1;
        [SerializeField] private UnityEvent<Collider> onHitEvent;
        [SerializeField] private UnityEvent onDeathEvent;

        private Rigidbody body;
        private new Collider collider;

        private int instigatorPlayerId;

        public Collider Collider => collider;
    
        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();

            if (lifeTime >= 0)
                StartCoroutine(DieAfterTime(lifeTime));
        }

        public void Fire()
        {
            Fire(initialVelocity);
        }

        public void Fire(float initialVelocity)
        {
            body.AddForce(initialVelocity * transform.forward, ForceMode.VelocityChange);
        }

        public void Fire(ref List<Collider> ignoreColliders)
        {
            foreach (Collider ignoreCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, ignoreCollider);
            }

            ignoreColliders.Add(collider);
            
            Fire();
        }
        
        public void Fire(float initialVelocity, ref List<Collider> ignoreColliders)
        {
            foreach (Collider ignoreCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, ignoreCollider);
            }

            ignoreColliders.Add(collider);
            
            Fire(initialVelocity);
        }

        private void OnTriggerEnter(Collider other)
        {
            onHitEvent?.Invoke(other);
            if (deathDelay > 0)
            {
                StartCoroutine(DieAfterTime(deathDelay));
            }
            else
                Die();
        }

        public void SetInstigator(int playerId)
        {
            instigatorPlayerId = playerId;
        }

        private IEnumerator DieAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            Die();
        }

        private void Die()
        {
            onDeathEvent?.Invoke();
            Destroy(gameObject);
        }

        public void SubscribeToDeathEvent(UnityAction response)
        {
            onDeathEvent.AddListener(response);
        }
    }
}

// private void OnTriggerEnter(Collider other)
// {
//     if (other.CompareTag("Player"))
//     {
//         if (ignoreInstigator && other.GetComponent<PlayerController>().GetPlayerId() == instigatorPlayerId) return;
//     
//         other.GetComponent<Damageable>().TakeDamage(damage);
//         Vector3 knockbackDir = new Vector3(body.velocity.x, 0f, body.velocity.z).normalized;
//         other.GetComponent<Knockbackable>().Knockback(knockbackDir * knockback);
//     }
//     else
//     {
//         Die();
//     }
// }
