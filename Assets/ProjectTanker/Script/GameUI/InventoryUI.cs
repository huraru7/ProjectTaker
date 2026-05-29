using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour, IDropHandler
{
    //:インベントリ一覧UI　Presenterからインベントリデータを受け取って表示する。開閉はEキーで行う。
    [SerializeField] private GameObject panel;
    [SerializeField] private InventoryItemUI itemPrefab;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private int minSlots = 5;

    private readonly Subject<int> _onModuleReturnedFromSlot = new();
    public Observable<int> OnModuleReturnedFromSlot => _onModuleReturnedFromSlot;

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            panel.SetActive(!panel.activeSelf);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (SlotItemUI.DraggingSlotIndex >= 0)
            _onModuleReturnedFromSlot.OnNext(SlotItemUI.DraggingSlotIndex);
    }

    /// <summary>
    /// インベントリ一覧を更新する　Presenterから呼ばれる
    /// </summary>
    public void UpdateDisplay(IReadOnlyList<ModuleData> inventory)
    {
        //:既存のアイテムをクリア
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        int count = Mathf.Max(inventory.Count, minSlots);
        for (int i = 0; i < count; i++)
        {
            InventoryItemUI item = Instantiate(itemPrefab, itemContainer);
            item.Setup(i < inventory.Count ? inventory[i] : null);
        }
    }

    private void OnDestroy() => _onModuleReturnedFromSlot.Dispose();
}
