using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    [SerializeField] private string title = default;
    [SerializeField] private int slotIndex = default;
    [SerializeField] private Sprite hudIcon = default;

    public string Title => title;
    public int SlotIndex => slotIndex;
    public Sprite HUDIcon => hudIcon;
}
