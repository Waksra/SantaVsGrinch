using UnityEngine;

namespace Gameplay
{
    public class EasySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject objectToSpawn;

        public void ProjectileSpawnObject()
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
        
        public void ProjectileSpawnObject(Collision collision)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}