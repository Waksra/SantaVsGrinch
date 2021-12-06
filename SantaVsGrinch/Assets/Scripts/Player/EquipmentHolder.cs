using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class EquipmentHolder : MonoBehaviour
    {
        private IEquippable equippable1;

        private int playerIndex;

        public int PlayerIndex => playerIndex;

        private void Awake()
        {
            playerIndex = GetComponent<PlayerInput>().playerIndex;
            
            equippable1?.Equip(this);
        }

        public void Activate(int index)
        {
            switch (index)
            {
                case 1:
                    equippable1?.OnActivate();
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
            }
        }
    }
}