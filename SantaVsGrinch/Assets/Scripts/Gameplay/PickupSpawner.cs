using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class PickupSpawner : MonoBehaviour
    {
        [SerializeField] private PickupChanceGroup[] pickups;
        [SerializeField, Range(0, 120)] private float initialMinimumTime = 5f;
        [SerializeField, Range(0, 120)] private float minTimeBetween = 2f;
        [SerializeField, Range(0, 120)] private float maxTimeBetween = 10f;
        [SerializeField, Range(1, 10)] private int maxPickups = 2;

        private SpawnPoint[] spawnPoints;

        private List<Pickup> spawnedPickups = new List<Pickup>();

        private float collectiveWeight;
        private float timeOfNextSpawn;

        private bool isSpawning;
        private bool stopSpawning;
        private bool atMax;

        private void Awake()
        {
            foreach (PickupChanceGroup pickupChanceGroup in pickups)
            {
                collectiveWeight += pickupChanceGroup.weight;
            }

            Transform[] points = GetComponentsInChildren<Transform>();
            spawnPoints = new SpawnPoint[points.Length - 1];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if(points[i] == transform)
                {
                    points[i] = points[spawnPoints.Length];
                    i--;
                    continue;
                }
                
                spawnPoints[i] = new SpawnPoint(points[i]);
            }
            
            StartCoroutine(SpawnCoroutine(false));
        }

        public void StopSpawning()
        {
            if(isSpawning)
                stopSpawning = true;
        }

        public void StartSpawning()
        {
            if (!isSpawning)
                StartCoroutine(SpawnCoroutine());
        }

        private IEnumerator SpawnCoroutine(bool skipInitialWait = true)
        {
            if(!skipInitialWait)
                yield return new WaitForSeconds(Random.Range(initialMinimumTime, maxTimeBetween));

            isSpawning = true;
            while (true)
            {
                if(stopSpawning)
                {
                    stopSpawning = false;
                    yield break;
                }

                int spawnId = GetAvailableSpawnPoint();

                if (spawnedPickups.Count < maxPickups && spawnId >= 0)
                    SpawnPickup(spawnId);
                else
                {
                    atMax = true;
                    yield return new WaitUntil(() => !atMax);
                }
                
                if(atMax)
                    yield return new WaitUntil(() => !atMax);

                yield return new WaitForSeconds(Random.Range(minTimeBetween, maxTimeBetween));
            }
        }

        private void SpawnPickup(int pointId)
        {
            Pickup chosenPickup = default;

            float random = Random.Range(0, collectiveWeight);
            float counter = 0;
            foreach (PickupChanceGroup pickupChanceGroup in pickups)
            {
                counter += pickupChanceGroup.weight;
                if (counter > random)
                {
                    chosenPickup = pickupChanceGroup.pickup;
                    break;
                }
            }

            Pickup newPickup = Instantiate(chosenPickup, spawnPoints[pointId].point.position, spawnPoints[pointId].point.rotation);
            newPickup.SubscribeToOnPickup(PickupResponse);
            spawnedPickups.Add(newPickup);

            spawnPoints[pointId].pickup = newPickup;
            spawnPoints[pointId].isAvailable = false;

            if (spawnedPickups.Count == maxPickups)
                atMax = true;

            timeOfNextSpawn = Random.Range(minTimeBetween, maxTimeBetween) + Time.time;
        }

        private int GetAvailableSpawnPoint()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].isAvailable)
                    ids.Add(i);
            }

            if (ids.Count == 0)
                return -1;

            return ids[Random.Range(0, ids.Count - 1)];
        }

        private void ReclaimSpawnPoint(Pickup pickup)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].pickup == pickup)
                {
                    spawnPoints[i].isAvailable = true;
                    return;
                }            }
        }

        private void PickupResponse(Pickup pickup)
        {
            spawnedPickups.Remove(pickup);
            ReclaimSpawnPoint(pickup);
            
            if(atMax)
                atMax = false;
        }

        [Serializable]
        private struct PickupChanceGroup
        {
            public Pickup pickup; 
            [Range(0, 10)] public float weight;
        }

        private struct SpawnPoint
        {
            public Transform point;
            public bool isAvailable;

            public Pickup pickup;

            public SpawnPoint(Transform newPoint, bool newIsAvailable = true)
            {
                point = newPoint;
                isAvailable = newIsAvailable;

                pickup = null;
            }
        }
    }
}