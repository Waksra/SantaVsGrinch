using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class OnDisableEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent onDisableEvent;

        private void Awake()
        {
            onDisableEvent ??= new UnityEvent();
        }

        public void SubscribeOnDisableEvent(UnityAction response)
        {
            onDisableEvent.AddListener(response);
        }

        public void UnsubscribeOnDisableEvent(UnityAction response)
        {
            onDisableEvent.RemoveListener(response);
        }
        
        private void OnDisable()
        {
            onDisableEvent?.Invoke();
        }
    }
}