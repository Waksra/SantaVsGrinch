using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class Damager : MonoBehaviour
    {
        private Collider myCollider;
        
        [SerializeField] private float damage = 1f;
        [FoldoutGroup("Radial")]
        [SerializeField, Range(0, 50)] private float radius = 10f;
        [FoldoutGroup("Radial")]
        [SerializeField] private AnimationCurve radialDamageCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [FoldoutGroup("Radial")]
        [SerializeField] private LayerMask layerMask = default;

        private void Awake()
        {
            myCollider = GetComponent<Collider>();
        }

        public void ProjectileDealDamage(Collider other)
        {
            if(other.TryGetComponent(out Damageable damageable))
                DealDamage(damageable);
        }
        
        public void DealDamage(Damageable damageable)
        {
            damageable.TakeDamage(damage);
        }
        
        public void DealDamageRadial()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask.value);
            if (colliders.Length <= 0) return;

            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i] == myCollider) continue;
                
                float distance = Vector3.Distance(colliders[i].transform.position, transform.position);
                float damageMagnitude = damage * radialDamageCurve.Evaluate(distance / radius);
                colliders[i].GetComponent<Damageable>().TakeDamage(damageMagnitude);
            }
        }
    }
}