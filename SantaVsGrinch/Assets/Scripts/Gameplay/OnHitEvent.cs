using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class OnHitEvent : MonoBehaviour
    {
        private UnityEvent<Collider> onTrigger;
        private UnityEvent<Collision> OnCollision;
        
        private void OnTriggerEnter(Collider other)
        {
            onTrigger?.Invoke(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision?.Invoke(collision);
        }
    }
}