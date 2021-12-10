using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class OnHitEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider> onTrigger;
        [SerializeField] private UnityEvent<Collision> OnCollision;
        
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