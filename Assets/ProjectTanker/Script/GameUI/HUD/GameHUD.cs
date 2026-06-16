using LitMotion;
using LitMotion.Extensions;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectTanker.UI;

public class GameHUD : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private TankStatus       tankStatus;
    [SerializeField] private TankBulletManager bulletManager;
    [SerializeField] private ThemeColor        theme;

    [Header("HP")]
    [SerializeField] private Image            hpFill;
    [SerializeField] private TextMeshProUGUI  hpText;
    [SerializeField] private RectTransform    _hpBarRoot;

    [Header("弾数")]
    [SerializeField] private Image            crosshairIcon;
    [SerializeField] private Image            reloadCircle;
    [SerializeField] private TextMeshProUGUI  ammoText;
    [SerializeField] private GameObject       fullBadge;
    [SerializeField] private Sprite           crosshairFullSprite;
    [SerializeField] private Sprite           crosshairEmptySprite;

    [Header("レベル")]
    [SerializeField] private TextMeshProUGUI  levelText;

    private int           _lastHP;
    private MotionHandle  _fullBadgeHandle;
    private RectTransform _fullBadgeRt;

    void Start()
    {
        _lastHP = tankStatus.getHP.Value;

        tankStatus.getHP.Subscribe(hp =>
        {
            if (hp < _lastHP) ShakeHPBar();
            _lastHP = hp;
            RefreshHP(hp, tankStatus.getMaxHP.Value);
        }).AddTo(this);
        tankStatus.getMaxHP.Subscribe(max => RefreshHP(tankStatus.getHP.Value, max)).AddTo(this);

        bulletManager.getTotalRounds.Subscribe(RefreshAmmo).AddTo(this);

        _fullBadgeRt = fullBadge != null ? fullBadge.GetComponent<RectTransform>() : null;
        if (fullBadge != null) fullBadge.SetActive(false);

        if (reloadCircle != null) reloadCircle.gameObject.SetActive(false);

        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnXpChanged
                .Subscribe(_ => RefreshXp())
                .AddTo(this);
            RefreshXp();
        }

        RefreshAmmo(bulletManager.getTotalRounds.Value);
    }

    void Update()
    {
        if (reloadCircle == null || bulletManager == null) return;

        float progress = bulletManager.ReloadProgress;
        bool isReloading = progress > 0f;
        reloadCircle.gameObject.SetActive(isReloading);
        if (isReloading)
            reloadCircle.fillAmount = progress;
    }

    private void RefreshHP(int hp, int max)
    {
        if (hpFill == null) return;
        float ratio = max > 0 ? (float)hp / max : 0f;
        hpFill.fillAmount = ratio;

        if (theme != null)
            hpFill.color = Color.Lerp(theme.fire, theme.wind, ratio);

        if (hpText != null)
            hpText.text = $"{hp} / {max}";
    }

    private void RefreshAmmo(int rounds)
    {
        int max = tankStatus.getMagazineCapacity.Value;

        if (ammoText != null)
            ammoText.text = rounds.ToString();

        if (crosshairIcon != null && crosshairFullSprite != null && crosshairEmptySprite != null)
            crosshairIcon.sprite = rounds > 0 ? crosshairFullSprite : crosshairEmptySprite;

        bool isFull = max > 0 && rounds >= max;
        if (fullBadge != null)
        {
            if (isFull && !fullBadge.activeSelf)       ShowFullBadge();
            else if (!isFull && fullBadge.activeSelf)  fullBadge.SetActive(false);
        }
    }

    private void ShowFullBadge()
    {
        if (_fullBadgeHandle.IsActive()) _fullBadgeHandle.Cancel();
        if (_fullBadgeRt != null) _fullBadgeRt.localScale = Vector3.zero;
        fullBadge.SetActive(true);
        _fullBadgeHandle = LMotion.Create(0f, 1f, 0.2f)
            .WithEase(Ease.OutBack)
            .Bind(s => { if (_fullBadgeRt != null) _fullBadgeRt.localScale = Vector3.one * s; })
            .AddTo(this);
    }

    private void ShakeHPBar()
    {
        if (_hpBarRoot == null) return;
        LMotion.Shake.Create(_hpBarRoot.anchoredPosition, new Vector2(3f, 0f), 0.3f)
            .WithFrequency(8)
            .WithDampingRatio(1f)
            .BindToAnchoredPosition(_hpBarRoot)
            .AddTo(this);
    }

    private void RefreshXp()
    {
        if (ExperienceManager.Instance == null) return;
        if (levelText != null)
            levelText.text = $"Lv.{ExperienceManager.Instance.CurrentLevel}";
    }
}
