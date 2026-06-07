using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private SlotItemUI[] slotItems;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            panel.SetActive(!panel.activeSelf);
    }

    public void UpdateDisplay(IReadOnlyList<ModuleData> slots)
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            ModuleData data = i < slots.Count ? slots[i] : null;
            slotItems[i].Setup(data, i);
        }
    }
}
