using Player;
using UnityEngine;

namespace Gameplay
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private GameObject gunPickup;
        [SerializeField, Range(1, 2)] private int slot = 1;
        [SerializeField] private LayerMask layerMask = 128;

        private bool isValidGun;

        private void Awake()
        {
            if (gunPickup.TryGetComponent(out IEquippable equippable))
                isValidGun = true;
            else
                Debug.LogError($"{name} pickup has no valid gun set.");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isValidGun)
                return;
            
            if (layerMask == (layerMask | (1 << other.gameObject.layer)) && other.TryGetComponent(out EquipmentHolder holder))
            {
                holder.Equip(gunPickup, slot);
                Destroy(gameObject);
            }
        }
    }
}