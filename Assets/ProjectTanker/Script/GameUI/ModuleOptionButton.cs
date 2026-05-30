using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleOptionButton : MonoBehaviour
{
    //:モジュール選択肢1つ分のUI
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Setup(ModuleData data, Action onSelect)
    {
        iconImage.sprite = data.icon;
        nameText.text = data.moduleName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelect?.Invoke());
    }
}
