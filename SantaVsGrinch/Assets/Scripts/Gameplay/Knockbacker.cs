using UnityEngine;

namespace Gameplay
{
    public class Knockbacker : MonoBehaviour
    {
        [SerializeField, Range(0, 50)] private float knockback = 10f;
        
        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }
        

        public void ProjectileKnockback(Collision collision)
        {
            if(!collision.collider.TryGetComponent(out Knockbackable knockbackable))
                return;

            Vector3 velocity = body.velocity;
            Vector3 knockbackDir = new Vector3(velocity.x, 0f, velocity.z).normalized; 
            knockbackable.Knockback(knockbackDir * knockback);
        }
    }
}