using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    //:インベントリアイテム1つ分のUI
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public ModuleData Data { get; private set; } //:ドラッグドロップ実装時に参照する

    public void Setup(ModuleData data)
    {
        Data = data;
        iconImage.sprite = data.icon;
        nameText.text = data.moduleName;
    }
}
