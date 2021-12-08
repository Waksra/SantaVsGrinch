using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class ParticleCollection : MonoBehaviour
    {
        [SerializeField] private UnityEvent onDisableEvent;
        
        private int disabledSystems;

        private ParticleSystem[] systems;
        
        private void Start()
        {
            systems = GetComponentsInChildren<ParticleSystem>(true);
            StartCoroutine(DelayedSubscribe());
        }

        private void OnEnable()
        {
            if(systems != null)
                foreach (ParticleSystem system in systems)
                {
                    system.gameObject.SetActive(true);
                }
        }

        private IEnumerator DelayedSubscribe()
        {
            yield return null;
            
            foreach (ParticleSystem system in systems)
            {
                OnDisableEvent disableEvent = system.gameObject.AddComponent<OnDisableEvent>();
                disableEvent.SubscribeOnDisableEvent(OnParticleStopped);
                
                if (!system.gameObject.activeSelf)
                    disabledSystems++;
            }
        }
        
        public void SubscribeOnDisableEvent(UnityAction response)
        {
            onDisableEvent.AddListener(response);
        }

        public void UnsubscribeOnDisableEvent(UnityAction response)
        {
            onDisableEvent.RemoveListener(response);
        }

        private void OnParticleStopped()
        {
            disabledSystems++;
            if (disabledSystems == systems.Length)
            {
                disabledSystems = 0;
                StartCoroutine(DelayedDisable());
            }
        }

        private void OnDisable()
        {
            
        }

        private IEnumerator DelayedDisable()
        {
            yield return null;
            gameObject.SetActive(false);
        }
    }
}