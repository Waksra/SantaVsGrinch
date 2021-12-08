using Managers;
using UnityEngine;

namespace Gameplay
{
    public class Poolable : MonoBehaviour
    {
        [SerializeField] private int initialPoolSize = 5;
        [SerializeField] private bool repoolOnDisable = true;
        
        public int PoolIndex { get; set; } = -1;
        public int InitialPoolSize => initialPoolSize;

        public void Repool()
        {
            if(PoolIndex != -1)
                ObjectPooler.ReturnToPool(gameObject, PoolIndex);
            else
                Debug.LogError($"{name} has not been assigned a pool.");
        }

        private void OnDisable()
        {
            if(repoolOnDisable && PoolIndex != -1)
                Repool();
        }
    }
}