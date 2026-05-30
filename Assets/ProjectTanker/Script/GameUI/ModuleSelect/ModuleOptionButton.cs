using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectTanker.UI;

public class ModuleOptionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image accentBar;   // カード上部の属性カラーバー
    [SerializeField] private ThemeColor theme;

    public void Setup(ModuleData data, Action onSelect)
    {
        iconImage.sprite = data.icon;
        nameText.text = data.moduleName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelect?.Invoke());

        if (accentBar != null && theme != null)
            accentBar.color = GetElementColor(data.moduleElement);
    }

    private Color GetElementColor(ModuleElementEnum element) => element switch
    {
        ModuleElementEnum.Earth => theme.earth,
        ModuleElementEnum.Water => theme.water,
        ModuleElementEnum.Fire  => theme.fire,
        ModuleElementEnum.Wind  => theme.wind,
        _                       => theme.none,
    };
}
