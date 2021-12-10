using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private GameObject gunPickup;
        [SerializeField, Range(1, 2)] private int slot = 1;
        [SerializeField] private LayerMask layerMask = 128;

        [SerializeField] private UnityEvent<Pickup> onPickup;

        private bool isValidGun;

        private void Awake()
        {
            if (gunPickup.TryGetComponent(out IEquippable equippable))
                isValidGun = true;
            else
                Debug.LogError($"{name} pickup has no valid gun set.");
        }

        public void SubscribeToOnPickup(UnityAction<Pickup> callback)
        {
            onPickup.AddListener(callback);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isValidGun)
                return;
            
            if (layerMask == (layerMask | (1 << other.gameObject.layer)) && other.TryGetComponent(out EquipmentHolder holder))
            {
                holder.Equip(gunPickup, slot);
                onPickup?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}