using Managers;
using UnityEngine;

namespace Gameplay
{
    public class EasySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject objectToSpawn;

        private bool isPoolable;
        private int poolIndex;

        private void Awake()
        {
            if (objectToSpawn.TryGetComponent(out Poolable poolable))
            {
                isPoolable = true;
                poolIndex = ObjectPooler.AddObject(objectToSpawn);
                poolable.PoolIndex = poolIndex;
            }
        }

        public void SpawnObject()
        {
            if (isPoolable)
            {
                GameObject go = ObjectPooler.GetObject(poolIndex);
                go.transform.position = transform.position;
                go.transform.rotation = Quaternion.identity;
                go.SetActive(true);
            }
            else
                Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }

        public void SpawnObject(Transform location)
        {
            if (isPoolable)
            {
                GameObject go = ObjectPooler.GetObject(poolIndex);
                go.transform.position = location.position;
                go.transform.rotation = location.rotation;
                go.SetActive(true);
            }
            else
                Instantiate(objectToSpawn, location.position, location.rotation);
        }
        
        public void SpawnObject(Collider other)
        {
            if (isPoolable)
            {
                GameObject go = ObjectPooler.GetObject(poolIndex);
                go.transform.position = transform.position;
                go.transform.rotation = Quaternion.identity;
                go.SetActive(true);
            }
            else
                Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}