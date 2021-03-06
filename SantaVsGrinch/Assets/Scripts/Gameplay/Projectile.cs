using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField, Range(0, 50)] private float initialVelocity = 20f;
        [SerializeField, Range(0, 2)] private float gravityScale = 1f;
        [SerializeField] private LayerMask hitMask = 1920;
        [Space(10)]
        [SerializeField, Range(0, 5)] private float deathDelay = 0f;
        [SerializeField] private bool ignoreInstigator = true;
        [SerializeField, Range(-1, 60)] private float lifeTime = -1;
        [SerializeField, Range(1, 100)] private int initialPoolAmount = 10;
        [Space(10)]
        [SerializeField] private UnityEvent<Collider> onHitEvent;
        [SerializeField] private UnityEvent onDeathEvent;

        [SerializeField] private AudioClip onHitClip = default;
        
        private Rigidbody body;
        private new Collider collider;
        private TrailRenderer trail;

        private int instigatorPlayerId;

        public Collider Collider => collider;
        public int InitialPoolAmount => initialPoolAmount;

        public int PoolIndex { get; set; } = -1;
    
        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.useGravity = false;
            collider = GetComponent<Collider>();
            trail = GetComponentInChildren<TrailRenderer>();

            if (lifeTime >= 0)
                StartCoroutine(DieAfterTime(lifeTime));
        }

        private void FixedUpdate()
        {
            body.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
        }

        public void Reset()
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            if(trail != null)
                trail.Clear();
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
            if ((hitMask & (1 << other.gameObject.layer)) == 0)
                return;
            
            if (other.TryGetComponent(out PlayerInput player) && player.playerIndex == instigatorPlayerId)
                return;
            if(other.TryGetComponent(out Projectile otherProjectile) && otherProjectile.instigatorPlayerId == instigatorPlayerId)
                return;
            
            onHitEvent?.Invoke(other);
            if (onHitClip != null)
                SoundManager.PlaySFXRandomized(onHitClip, transform.position);
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
            if(PoolIndex == -1)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }

        public void SubscribeToDeathEvent(UnityAction response)
        {
            onDeathEvent.AddListener(response);
        }

        private void OnDisable()
        {
            ProjectilePooler.ReturnToPool(this, PoolIndex);
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
