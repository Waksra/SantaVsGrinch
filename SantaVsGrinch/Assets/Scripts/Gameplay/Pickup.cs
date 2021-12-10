using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private GameObject gunPickup;
        [SerializeField] private LayerMask layerMask = 128;

        [SerializeField] private UnityEvent<Pickup> onPickup;

        private int slot;

        private void Awake()
        {
            if (gunPickup.TryGetComponent(out IEquippable equippable))
                slot = gunPickup.GetComponent<WeaponInfo>().SlotIndex;
            else
                Debug.LogError($"{name} pickup has no valid gun set.");
        }

        public void SubscribeToOnPickup(UnityAction<Pickup> callback)
        {
            onPickup.AddListener(callback);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (layerMask == (layerMask | (1 << other.gameObject.layer)) && other.TryGetComponent(out EquipmentHolder holder))
            {
                holder.Equip(gunPickup, slot);
                onPickup?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}