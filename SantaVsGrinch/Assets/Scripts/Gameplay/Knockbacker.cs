using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class Knockbacker : MonoBehaviour
    {
        private Collider myCollider;
        
        [SerializeField, Range(-500, 500)] private float knockback = 100f;
        
        [FoldoutGroup("Radial")]
        [SerializeField, Range(0, 50)] private float radius = 10f;
        [FoldoutGroup("Radial")]
        [SerializeField] private AnimationCurve radialKnockbackCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [FoldoutGroup("Radial")]
        [SerializeField] private LayerMask layerMask = default;
        
        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            myCollider = GetComponent<Collider>();
        }

        public void KnockbackAlongVelocity(Collider other)
        {
            if(!other.TryGetComponent(out Knockbackable knockbackable))
                return;

            Vector3 velocity = body.velocity;
            Vector3 knockbackDir = new Vector3(velocity.x, 0f, velocity.z).normalized; 
            knockbackable.Knockback(knockbackDir * knockback);
        }
        
        public void KnockbackAlongVelocity(Collision collision)
        {
            if(!collision.gameObject.TryGetComponent(out Knockbackable knockbackable))
                return;

            Vector3 velocity = body.velocity;
            Vector3 knockbackDir = new Vector3(velocity.x, 0f, velocity.z).normalized; 
            knockbackable.Knockback(knockbackDir * knockback);
        }

        public void KnockbackAway(Collider other)
        {
            if(!other.TryGetComponent(out Knockbackable knockbackable))
                return;

            Vector3 direction = other.transform.position - body.position;
            Vector3 knockbackDir = new Vector3(direction.x, 0f, direction.z).normalized; 
            knockbackable.Knockback(knockbackDir * knockback);
        }
        
        public void KnockbackAway(Collision collision)
        {
            if(!collision.gameObject.TryGetComponent(out Knockbackable knockbackable))
                return;

            Vector3 direction = collision.transform.position - body.position;
            Vector3 knockbackDir = new Vector3(direction.x, 0f, direction.z).normalized; 
            knockbackable.Knockback(knockbackDir * knockback);
        }

        public void KnockbackRadial()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask.value);
            if (colliders.Length <= 0) return;

            foreach (Collider collider in colliders)
            {
                if (collider == myCollider) continue;
                
                Vector3 vector = collider.transform.position - transform.position;
                float knockbackMagnitude = knockback * radialKnockbackCurve.Evaluate(vector.magnitude / radius);
                Vector3 dir = new Vector3(vector.x, 0f, vector.z).normalized;
                collider.GetComponent<Knockbackable>().Knockback(dir * knockbackMagnitude);
            }
        }
    }
}