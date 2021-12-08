using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class ProjectileGun : MonoBehaviour, IEquippable
    {
        [SerializeField] private Projectile projectile;
        [SerializeField, Range(0, 3)] private float inputBuffer;
        [SerializeField, Range(0, 60)] private float shotsPerSecond;
        [SerializeField, Range(0, 20)] private float knockback;

        [SerializeField, FoldoutGroup("Firing Modes")] private bool isAutomatic;
        [SerializeField, FoldoutGroup("Firing Modes")] private bool isMultiShot;
        [SerializeField, FoldoutGroup("Firing Modes")] private bool isBurst;

        [ShowIfGroup("isMultiShot")]
        [FoldoutGroup("isMultiShot/Multi Shot")]
        [SerializeField, Range(0, 360)] private float spread;
        [FoldoutGroup("isMultiShot/Multi Shot")]
        [SerializeField, Range(0, 30)] private int multiProjectiles = 1;

        [ShowIfGroup("isBurst")] 
        [FoldoutGroup("isBurst/Burst")] 
        [SerializeField, Range(1, 20)] private int burstProjectiles;
        [FoldoutGroup("isBurst/Burst")] 
        [SerializeField, Range(0, 3)] private float burstDelay;

        private new Transform transform;
        private Knockbackable knockbackable;
        private EquipmentHolder owner;

        //General
        private float timeBetweenShots;
        private float timeOfNextShot;
        private List<Collider> firedColliders = new List<Collider>();
        private int poolIndex;
        private bool delayedFireRunning;

        //Auto
        private bool isAutoing;
        private Coroutine autoRoutine;
        private bool hasFired;
        private bool endAuto;
        
        //Multishot
        private float angleSpacing;
        
        //Burst
        private bool isBursting;

        private void OnValidate()
        {
            timeBetweenShots = 1 / shotsPerSecond;
            angleSpacing = spread / multiProjectiles;
        }

        private void Awake()
        {
            OnValidate();
            transform = GetComponent<Transform>();
        }

        private void Start()
        {
            poolIndex = ProjectilePooler.GetIndex(projectile.name);
            if (poolIndex == -1)
                poolIndex = ProjectilePooler.AddProjectile(projectile);
        }

        public void Equip(EquipmentHolder owner)
        {
            this.owner = owner;
            knockbackable = owner.GetComponent<Knockbackable>();
        }

        public void OnFireInput(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnActivate();
            else if(context.canceled)
                OnDeactivate();
        }

        public void OnActivate()
        {
            if ((timeOfNextShot <= Time.time || isAutomatic) && !isBursting)
                ShootWithFireMode();
            else if(timeOfNextShot - Time.time < inputBuffer && !delayedFireRunning)
                StartCoroutine(DelayedFire(timeOfNextShot - Time.time));
        }

        public void OnDeactivate()
        {
            if(isAutomatic && isAutoing)
                StopAuto();
            
        }

        private IEnumerator DelayedFire(float delay)
        {
            delayedFireRunning = true;
            yield return new WaitForSeconds(delay);
            
            ShootWithFireMode();
            delayedFireRunning = false;
        }

        private void ShootWithFireMode()
        {
            if(isAutomatic)
                StartAuto();
            else if(isBurst)
                FireBurst();
            else
                Fire();
        }

        private void StartAuto()
        {
            if(!isAutoing)
                autoRoutine = StartCoroutine(AutoRoutine());
            else if (isAutoing && endAuto)
                endAuto = false;
        }

        private void StopAuto()
        {
            if(!isAutoing)
                return;
            if (!hasFired)
                endAuto = true;
            else
            {
                StopCoroutine(autoRoutine);
                isAutoing = false;
            }
        }

        private void Fire()
        {
            Vector3 forward = transform.forward;

            if(!isBursting)
                timeOfNextShot = Time.time + timeBetweenShots;
            
            if (!isMultiShot)
            {
                FireProjectileDirection(forward);

                knockbackable.Knockback(-forward * knockback);
            }
            else
            {
                float startAngle = -(spread / 2);
                for (int i = 0; i < multiProjectiles; i++)
                {
                    Vector3 fireDirection = Quaternion.Euler(0, startAngle + angleSpacing * i, 0) * forward;
                    FireProjectileDirection(fireDirection);
                    
                }
                
                knockbackable.Knockback(-forward * knockback);
            }
        }

        private void FireBurst()
        {
            timeOfNextShot = Time.time + timeBetweenShots * (burstProjectiles - 1) + burstDelay;
            StartCoroutine(BurstRoutine());
        }

        private void FireProjectileDirection(Vector3 direction)
        {
            Projectile newProjectile = ProjectilePooler.GetProjectile(poolIndex);

            newProjectile.transform.position = transform.position + direction * 1.5f;
            newProjectile.transform.rotation = Quaternion.LookRotation(direction);
            newProjectile.gameObject.SetActive(true);
            
            if(owner)
                newProjectile.SetInstigator(owner.PlayerIndex);
            
            newProjectile.Fire(ref firedColliders);
            newProjectile.SubscribeToDeathEvent(() => RemoveFromColliders(newProjectile.Collider));
        }

        private IEnumerator AutoRoutine()
        {
            isAutoing = true;
            hasFired = false;

            if (timeOfNextShot > Time.time)
                yield return new WaitForSeconds(timeOfNextShot - Time.time);
            

            WaitForSeconds shotGap = new WaitForSeconds(isBurst ? burstDelay : timeBetweenShots);

            while (true)
            {
                if(isBurst)
                    FireBurst();
                else
                    Fire();
                
                hasFired = true;
                
                yield return shotGap;

                if (endAuto)
                {
                    isAutoing = false;
                    endAuto = false;
                    yield break;
                }
            }
        }

        private IEnumerator BurstRoutine()
        {
            isBursting = true;
            
            int shotsFired = 0;
            WaitForSeconds shotGap = new WaitForSeconds(timeBetweenShots);

            while (shotsFired < burstProjectiles)
            {
                Fire();
                shotsFired++;
                yield return shotGap;
            }

            isBursting = false;
        }

        private void RemoveFromColliders(Collider coll)
        {
            firedColliders.Remove(coll);
        }
    }
}

// private Projectile FireProjectileDirection(Vector3 direction, List<Collider> ignore)
// {
//     Projectile newProjectile = FireProjectileDirection(direction);
//     Collider projectileCollider = newProjectile.GetComponent<Collider>();
//
//     foreach (Collider coll in ignore)
//     {
//         Physics.IgnoreCollision(projectileCollider, coll);
//     }
//
//     return newProjectile;
// }

// private void Fire(ref List<Collider> colliders)
// {
//     Vector3 forward = transform.forward;
//     
//     if (!isMultiShot)
//     {
//         FireProjectileDirection(forward);
//
//         knockbackable.Knockback(-forward * knockback);
//     }
//     else
//     {
//         float startAngle = -(spread / 2);
//         for (int i = 0; i < multiProjectiles; i++)
//         {
//             Vector3 fireDirection = Quaternion.Euler(0, startAngle + angleSpacing * i, 0) * forward;
//             Projectile newProjectile = FireProjectileDirection(fireDirection, colliders);
//             
//             colliders.Add(newProjectile.GetComponent<Collider>());
//         }
//         
//         knockbackable.Knockback(-forward * knockback);
//     }
// }