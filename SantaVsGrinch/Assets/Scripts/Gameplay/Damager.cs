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
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
            if (colliders.Length <= 0) return;

            foreach (var collider in colliders)
            {
                if (collider == myCollider) return;
                
                Vector3 vector = collider.transform.position - transform.position;
                float damageMagnitude = damage * radialDamageCurve.Evaluate(vector.magnitude / radius);
                collider.GetComponent<Damageable>().TakeDamage(damageMagnitude);
            }
        }
    }
}