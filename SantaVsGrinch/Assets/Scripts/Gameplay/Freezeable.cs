using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class Freezeable : MonoBehaviour
    {
        private Rigidbody body;

        // [SerializeField] private bool onAwake = false;
        [SerializeField] private bool useRealTime = true;
        [SerializeField] private AnimationCurve freezeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        
        [SerializeField] private UnityEvent frozenEvent;
        [SerializeField] private UnityEvent unfrozenEvent;

        private Vector3 savedVelocity = Vector3.zero;
    
        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        
            // if (onAwake)
            //     Freeze(freezeTime);
        }

        public void Freeze(float time)
        {
            if (body.isKinematic)
                StopAllCoroutines();
            else
                PerformFreeze();

            if (GetComponent<Damageable>().smashMode) 
                StartCoroutine(FreezeForTimeSmash(time));
        }

        public void FreezeDamage(float damage)
        {
            if (!gameObject.activeSelf) return;
            PerformFreeze();
            StartCoroutine(FreezeForTimeDamage(damage));
        }

        public void Unfreeze()
        {
            StopAllCoroutines();
            PerformUnfreeze();
        }

        private void PerformFreeze()
        {
            // if (body == null)
            //     body = GetComponent<Rigidbody>();
            savedVelocity = body.velocity;
            body.velocity = Vector3.zero;
            body.isKinematic = true;
            frozenEvent?.Invoke();
        }

        private void PerformUnfreeze()
        {
            body.isKinematic = false;
            body.velocity = savedVelocity;
            unfrozenEvent?.Invoke();
        }

        private float time;
        private IEnumerator FreezeForTimeDamage(float damage)
        {
            time = freezeCurve.Evaluate(damage * 0.01f);

            if (useRealTime)
                yield return new WaitForSecondsRealtime(time);
            else
                yield return new WaitForSeconds(time);

            PerformUnfreeze();
        }
        
        private IEnumerator FreezeForTimeSmash(float time = -1f)
        {
            time = freezeCurve.Evaluate(GetComponent<Damageable>().Health * 0.01f);

            if (useRealTime)
                yield return new WaitForSecondsRealtime(time);
            else
                yield return new WaitForSeconds(time);

            PerformUnfreeze();
        }
    }
}
