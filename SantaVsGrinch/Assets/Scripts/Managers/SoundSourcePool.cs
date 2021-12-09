using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class SoundSourcePool
    {
        private int refillAmount;

        private GameObject parent;
        private GameObject pool;

        private List<AudioSource> pooledSources;

        public SoundSourcePool(GameObject parent, int initialSources, int refillAmount)
        {
            this.refillAmount = refillAmount;
            this.parent = parent;
            pooledSources = new List<AudioSource>();

            InitializePool(initialSources);
        }

        public AudioSource GetSource()
        {
            if (pooledSources.Count == 0)
                pooledSources.AddRange(CreatNewSources(refillAmount));

            AudioSource source = pooledSources[0];
            pooledSources.RemoveAt(0);

            source.gameObject.SetActive(true);
            return source;
        }

        public void RepoolSource(AudioSource source)
        {
            ResetSource(source);
            pooledSources.Add(source);
        }

        private void InitializePool(int amount)
        {
            if (pool != null)
                Object.Destroy(pool);

            pool = new GameObject("Source Pool");
            pool.transform.parent = parent.transform;

            pooledSources.AddRange(CreatNewSources(amount));
        }

        private AudioSource CreateNewSource()
        {
            GameObject newGO = new GameObject($"Source{pooledSources.Count}");
            newGO.transform.parent = pool.transform;

            AudioSource newSource = newGO.AddComponent<AudioSource>();
            newGO.SetActive(false);
            return newSource;
        }

        private AudioSource[] CreatNewSources(int amount)
        {
            AudioSource[] newSources = new AudioSource[amount];
            for (int i = 0; i < amount; i++)
            {
                newSources[i] = CreateNewSource();
            }

            return newSources;
        }

        private void ResetSource(AudioSource source)
        {
            source.gameObject.SetActive(false);
            source.gameObject.transform.parent = pool.transform;
            source.clip = null;
            source.loop = false;
            source.mute = false;
            source.pitch = 1;
            source.volume = 1;
        }
    }
}