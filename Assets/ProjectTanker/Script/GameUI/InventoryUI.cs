using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    //:インベントリ一覧UI　Presenterからインベントリデータを受け取って表示する。開閉はEキーで行う。
    [SerializeField] private GameObject panel;
    [SerializeField] private InventoryItemUI itemPrefab;
    [SerializeField] private Transform itemContainer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            panel.SetActive(!panel.activeSelf);
    }

    /// <summary>
    /// インベントリ一覧を更新する　Presenterから呼ばれる
    /// </summary>
    public void UpdateDisplay(IReadOnlyList<ModuleData> inventory)
    {
        //:既存のアイテムをクリア
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (ModuleData module in inventory)
        {
            InventoryItemUI item = Instantiate(itemPrefab, itemContainer);
            item.Setup(module);
        }
    }
}
