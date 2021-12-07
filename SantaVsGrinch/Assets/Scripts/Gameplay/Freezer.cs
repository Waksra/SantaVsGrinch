using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class Freezer : MonoBehaviour
    {
        private Collider myCollider;
        
        [SerializeField] private float freezeTime = 1f;
        [FoldoutGroup("Radial")]
        [SerializeField, Range(0, 50)] private float radius = 10f;
        [FoldoutGroup("Radial")]
        [SerializeField] private AnimationCurve radialFreezeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [FoldoutGroup("Radial")]
        [SerializeField] private LayerMask layerMask = default;
        
        public void Freeze(Collider other)
        {
            if (!other.TryGetComponent(out Freezeable freezeable)) return;
            freezeable.Freeze(freezeTime);
        }

        public void Unfreeze(Collider other)
        {
            if (!other.TryGetComponent(out Freezeable freezeable)) return;
            freezeable.Unfreeze();
        }
        
        public void FreezeRadial()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask.value);
            if (colliders.Length <= 0) return;

            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                if (colliders[i] == myCollider) continue;
                
                float distance = Vector3.Distance(colliders[i].transform.position, transform.position);
                float time = freezeTime * radialFreezeCurve.Evaluate(distance / radius);
                if (!colliders[i].TryGetComponent(out Freezeable freezeable)) continue;
                freezeable.Freeze(time);
            }
        }
    }
}