using UnityEngine;

namespace Gameplay
{
    public class Damager : MonoBehaviour
    {
        [SerializeField] private float damage = 1f;
        
        public void ProjectileDealDamage(Collision collision)
        {
            if(collision.collider.TryGetComponent(out Damageable damageable))
                DealDamage(damageable);
        }
        
        public void DealDamage(Damageable damageable)
        {
            damageable.TakeDamage(damage);
        }
    }
}