using System.Collections.Generic;
using R3;
using UnityEngine;

public class SlotUI : MonoBehaviour
{
    //:装備スロット一覧UI　Presenterからスロットデータを受け取って表示する。開閉はInventoryUIと同じパネル親で管理する。
    [SerializeField] private SlotItemUI[] slotItems; //:7つをInspectorで設定

    private readonly Subject<(int slotIndex, ModuleData module)> _onModuleDropped = new();
    public Observable<(int slotIndex, ModuleData module)> OnModuleDropped => _onModuleDropped;

    /// <summary>
    /// スロット一覧を更新する　Presenterから呼ばれる
    /// </summary>
    public void UpdateDisplay(IReadOnlyList<ModuleData> slots)
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            int capturedIndex = i;
            ModuleData data = i < slots.Count ? slots[i] : null;
            slotItems[i].Setup(data, capturedIndex, dropped => _onModuleDropped.OnNext((capturedIndex, dropped)));
        }
    }

    private void OnDestroy() => _onModuleDropped.Dispose();
}
