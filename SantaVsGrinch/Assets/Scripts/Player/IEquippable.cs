using System;
using UnityEngine;

namespace Player
{
    public interface IEquippable
    {
        bool IsActive { get; }
        void Equip(EquipmentHolder owner);

        void Unequip();

        void OnActivate();

        void OnDeactivate();
    }
}