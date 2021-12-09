using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class EquipmentHolder : MonoBehaviour
    {
        [SerializeField] private GameObject slot1;
        [SerializeField] private GameObject slot2;

        private new Transform transform;
        
        private IEquippable equippable1;
        private IEquippable equippable2;

        private HUDManager hudManager;

        private int playerIndex;

        public int PlayerIndex => playerIndex;

        private void Awake()
        {
            transform = GetComponent<Transform>();

            hudManager = FindObjectOfType<HUDManager>();
            
            playerIndex = GetComponent<PlayerInput>().playerIndex;

            if(slot1 != null)
            {
                Equip(slot1, 1);
            }
            
            if(slot2 != null)
            {
                Equip(slot2, 2);
            }
        }
        
        public void OnFireInput1(InputAction.CallbackContext context)
        {
            if(context.performed)
                Activate(1);
            else if(context.canceled)
                Deactivate(1);
        }
        
        public void OnFireInput2(InputAction.CallbackContext context)
        {
            if(context.performed)
                Activate(2);
            else if(context.canceled)
                Deactivate(2);
        }

        public void Activate(int index)
        {
            switch (index)
            {
                case 1:
                    equippable1?.OnActivate();
                    break;
                case 2:
                    equippable2?.OnActivate();
                    break;
            }
        }

        public void Deactivate(int index)
        {
            switch (index)
            {
                case 1:
                    equippable1?.OnDeactivate();
                    break;
                case 2:
                    equippable2?.OnDeactivate();
                    break;
            }
        }

        public void Equip(GameObject equippable, int index)
        {
            switch (index)
            {
               case 1:
               {
                   GameObject go = Instantiate(equippable, transform);
                   go.transform.localPosition = Vector3.zero;
                   go.transform.localRotation = Quaternion.identity;
                   equippable1 = go.GetComponent<IEquippable>();
                   equippable1?.Equip(this);

                   if (hudManager != null && go.TryGetComponent(out WeaponInfo weaponInfo))
                       hudManager.UpdateWeapon(playerIndex, weaponInfo);
                   
                   break;
               }
               case 2:
               {
                   GameObject go = Instantiate(equippable, transform);
                   go.transform.localPosition = Vector3.zero;
                   go.transform.localRotation = Quaternion.identity;
                   equippable2 = go.GetComponent<IEquippable>();
                   equippable2?.Equip(this);
                   
                   if (hudManager != null && go.TryGetComponent(out WeaponInfo weaponInfo))
                       hudManager.UpdateWeapon(playerIndex, weaponInfo);
                   
                   break;
               }
            }
        }
    }
}