using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotItemUI : MonoBehaviour, IDropHandler
{
    //:装備スロット1つ分のUI
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public ModuleData Data { get; private set; }

    private Action<ModuleData> _onDrop;

    public void Setup(ModuleData data, Action<ModuleData> onDrop = null)
    {
        Data = data;
        _onDrop = onDrop;
        bool isEmpty = data == null;
        iconImage.sprite = isEmpty ? null : data.icon;
        nameText.text = isEmpty ? "" : data.moduleName;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryItemUI.DraggingData != null)
            _onDrop?.Invoke(InventoryItemUI.DraggingData);
    }
}
