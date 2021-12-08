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

        private int playerIndex;

        public int PlayerIndex => playerIndex;

        private void Awake()
        {
            transform = GetComponent<Transform>();
            
            playerIndex = GetComponent<PlayerInput>().playerIndex;

            GameObject slot1Go = Instantiate(slot1, transform.position, Quaternion.identity);
            slot1Go.transform.parent = transform;
            equippable1 = slot1Go.GetComponent<IEquippable>();
            
            GameObject slot2Go = Instantiate(slot2, transform.position, Quaternion.identity);
            slot2Go.transform.parent = transform;
            equippable2 = slot2Go.GetComponent<IEquippable>();
            
            equippable1?.Equip(this);
            equippable2?.Equip(this);
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

        public void Equip(IEquippable equippable, int index)
        {
            switch (index)
            {
               case 1:
                   equippable1 = equippable;
                   equippable1.Equip(this);
                   break;
               case 2:
                   equippable2 = equippable;
                   equippable2.Equip(this);
                   break;
            }
        }
    }
}