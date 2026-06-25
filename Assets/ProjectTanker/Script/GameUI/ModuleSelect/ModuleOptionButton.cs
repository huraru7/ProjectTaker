using System;
using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ProjectTanker.UI;

public class ModuleOptionButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image accentBar;   // カード上部の属性カラーバー
    [SerializeField] private ThemeColor theme;
    [SerializeField] private CanvasGroup canvasGroup;

    private MotionHandle _motionHandle;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance?.PlayModuleHover();
    }

    public void Setup(ModuleData data, Action onSelect)
    {
        iconImage.sprite = data.icon;
        nameText.text = data.moduleName;
        if (descriptionText != null) descriptionText.text = data.description;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelect?.Invoke());
        button.interactable = true;

        if (accentBar != null && theme != null)
            accentBar.color = GetElementColor(data.moduleElement);
    }

    /// <summary>表示開始時のポップイン演出（delay秒待ってから再生）</summary>
    public void PlayIntro(float delay)
    {
        if (_motionHandle.IsActive()) _motionHandle.Cancel();
        transform.localScale = Vector3.one * 0.6f;
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        _motionHandle = LMotion.Create(0f, 1f, 0.35f)
            .WithDelay(delay)
            .WithEase(Ease.OutBack)
            .Bind(t =>
            {
                transform.localScale = Vector3.one * Mathf.Lerp(0.6f, 1f, t);
                if (canvasGroup != null) canvasGroup.alpha = t;
            })
            .AddTo(this);
    }

    /// <summary>選択された側の演出：軽くポップ拡大してからフェードアウト</summary>
    public void PlaySelected(Action onComplete)
    {
        AudioManager.Instance?.PlayModuleSelect();
        button.interactable = false;
        if (_motionHandle.IsActive()) _motionHandle.Cancel();

        _motionHandle = LMotion.Create(0f, 1f, 0.32f)
            .WithEase(Ease.OutCubic)
            .WithOnComplete(() => onComplete?.Invoke())
            .Bind(t =>
            {
                float scale = t < 0.5f
                    ? Mathf.Lerp(1f, 1.18f, t / 0.5f)
                    : Mathf.Lerp(1.18f, 1.4f, (t - 0.5f) / 0.5f);
                transform.localScale = Vector3.one * scale;
                if (canvasGroup != null && t > 0.4f)
                    canvasGroup.alpha = 1f - Mathf.Clamp01((t - 0.4f) / 0.6f);
            })
            .AddTo(this);
    }

    /// <summary>非選択側の演出：縮小しながらフェードアウト</summary>
    public void PlayDismiss()
    {
        button.interactable = false;
        if (_motionHandle.IsActive()) _motionHandle.Cancel();

        _motionHandle = LMotion.Create(0f, 1f, 0.2f)
            .WithEase(Ease.InQuad)
            .Bind(t =>
            {
                transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.85f, t);
                if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            })
            .AddTo(this);
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
