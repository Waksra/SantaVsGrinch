using UnityEngine;

namespace Gameplay
{
    public class EasySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject objectToSpawn;

        public void SpawnObject()
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
        
        public void SpawnObject(Collision collision)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}