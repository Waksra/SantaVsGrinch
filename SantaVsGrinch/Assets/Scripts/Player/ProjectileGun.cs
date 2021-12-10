using System.Collections;
using Gameplay;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player
{
    public class ProjectileGun : MonoBehaviour, IEquippable
    {
        [SerializeField] private Projectile projectile;
        [SerializeField, Range(0, 3)] private float inputBuffer;
        [SerializeField, Range(0, 100)] private float knockback;
        
        [Space]
        
        [SerializeField, Range(0, 60)] private float rps;
        [SerializeField, Range(0, 60)] private float bulletSpread;
        [SerializeField] private bool useAmmunition = true;
        [ShowIfGroup("useAmmunition")]
        [SerializeField, Range(1, 300)] private int ammunition = 1;

        [Space] 
        
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private UnityEvent<Transform> onShoot;

        [SerializeField, FoldoutGroup("Firing Modes")] private bool isAutomatic;
        [SerializeField, FoldoutGroup("Firing Modes")] private bool isMultiShot;
        [SerializeField, FoldoutGroup("Firing Modes")] private bool isBurst;
        
        [ShowIfGroup("isAutomatic")]
        
        [FoldoutGroup("isAutomatic/Automatic")]
        [SerializeField] private bool useRpsCurve;
        [ShowIfGroup("isAutomatic/Automatic/useRpsCurve")]
        [SerializeField] private AnimationCurve rpsCurve;
        [Space]
        [FoldoutGroup("isAutomatic/Automatic")]
        [SerializeField] private bool useSpreadCurve;
        [ShowIfGroup("isAutomatic/Automatic/useSpreadCurve")]
        [SerializeField] private AnimationCurve bulletSpreadCurve;

        [ShowIfGroup("isMultiShot")]
        [FoldoutGroup("isMultiShot/Multi Shot")]
        [SerializeField, Range(0, 360)] private float arc;
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
        private int poolIndex;
        private bool delayedFireRunning;
        private int currentAmmunition;

        //Auto
        private bool isAutoing;
        private Coroutine autoRoutine;
        private bool hasFired;
        private bool endAuto;
        private float autoSpread;
        private float autoRps;
        
        //Multishot
        private float angleSpacing;
        
        //Burst
        private bool isBursting;

        public bool IsActive => isAutoing;


        private void OnValidate()
        {
            CalculateTimeBetween();
            angleSpacing = arc / (multiProjectiles - 1);
        }

        private void CalculateTimeBetween()
        {
            timeBetweenShots = 1 / (isAutomatic && useRpsCurve ? autoRps : rps);
        }

        private void Awake()
        {
            OnValidate();
            transform = GetComponent<Transform>();
            knockbackable = GetComponent<Knockbackable>();
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
            currentAmmunition = ammunition;
        }

        public void Unequip()
        {
            Destroy(gameObject);
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
            }
            else
            {
                float startAngle = -(arc / 2);
                for (int i = 0; i < multiProjectiles; i++)
                {
                    Vector3 fireDirection = Quaternion.Euler(0, startAngle + angleSpacing * i, 0) * forward;
                    FireProjectileDirection(fireDirection);
                    
                }
            }
            
            knockbackable.Knockback(-forward * knockback);

            if (useAmmunition)
            {
                ammunition--;
                if (ammunition <= 0) 
                    owner.Unequip(this);
            }

            onShoot?.Invoke(transform);
            if(fireSound != null)
                SoundManager.PlaySFXRandomized(fireSound, transform.position);
        }

        private void FireBurst()
        {
            timeOfNextShot = Time.time + timeBetweenShots * (burstProjectiles - 1) + burstDelay;
            StartCoroutine(BurstRoutine());
        }

        private void FireProjectileDirection(Vector3 direction)
        {
            Projectile newProjectile = ProjectilePooler.GetProjectile(poolIndex);
            
            if(bulletSpread > 0)
            {
                float spread = isAutomatic && useSpreadCurve ? autoSpread : bulletSpread;
                direction = Quaternion.Euler(0, Random.Range(-spread / 2, spread / 2), 0) * direction;
            }

            newProjectile.transform.position = transform.position;// + direction * 1.5f
            newProjectile.transform.rotation = Quaternion.LookRotation(direction);
            newProjectile.gameObject.SetActive(true);
            
            if(owner)
                newProjectile.SetInstigator(owner.PlayerIndex);
            
            newProjectile.Fire();
        }

        private IEnumerator AutoRoutine()
        {
            isAutoing = true;
            hasFired = false;

            if (timeOfNextShot > Time.time)
                yield return new WaitForSeconds(timeOfNextShot - Time.time);
            

            WaitForSeconds shotGap = new WaitForSeconds(isBurst ? burstDelay : timeBetweenShots);
            float startTime = Time.time;

            while (true)
            {
                if (useRpsCurve)
                {
                    float rpsCurveEvaluation = rpsCurve.Evaluate(Time.time - startTime);
                    autoRps = rpsCurveEvaluation * rps;
                    CalculateTimeBetween();
                    
                    shotGap = new WaitForSeconds(isBurst ? burstDelay * rpsCurveEvaluation : timeBetweenShots);
                }
                if (useSpreadCurve)
                    autoSpread = bulletSpreadCurve.Evaluate(Time.time - startTime) * bulletSpread;
                
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
    }
}