using System;
using UnityEngine;

namespace Player
{
    public interface IEquippable
    {
        void Equip(EquipmentHolder owner);
        
        void OnActivate();

        void OnDeactivate();
    }
}