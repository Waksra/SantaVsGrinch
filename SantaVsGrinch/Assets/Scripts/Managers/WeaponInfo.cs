using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    [SerializeField] private string title = default;
    [SerializeField, Range(1, 2)] private int slotIndex = 1;
    [SerializeField] private Sprite hudIcon = default;

    public string Title => title;
    public int SlotIndex => slotIndex;
    public Sprite HUDIcon => hudIcon;
}
