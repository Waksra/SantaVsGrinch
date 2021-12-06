using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class Knockbacker : MonoBehaviour
    {
        [SerializeField, Range(-50, 50)] private float knockback = 10f;
        
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
        }

        public void KnockbackAlongVelocity(Collision collision)
        {
            if(!collision.collider.TryGetComponent(out Knockbackable knockbackable))
                return;

            Vector3 velocity = body.velocity;
            Vector3 knockbackDir = new Vector3(velocity.x, 0f, velocity.z).normalized; 
            knockbackable.Knockback(knockbackDir * knockback);
        }

        public void KnockbackRadial()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
            if (colliders.Length <= 0) return;

            foreach (var collider in colliders)
            {
                Vector3 vector = collider.transform.position - transform.position;
                float knockbackMagnitude = knockback * radialKnockbackCurve.Evaluate(vector.magnitude / radius);
                Vector3 dir = new Vector3(vector.x, 0f, vector.z).normalized;
                collider.GetComponent<Knockbackable>().Knockback(dir * knockbackMagnitude);
            }
        }
    }
}