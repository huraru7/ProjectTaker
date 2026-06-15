using System.Collections;
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
    [SerializeField] private TankStatus tankStatus;
    [SerializeField] private TankBulletManager bulletManager;
    [SerializeField] private ThemeColor theme;

    [Header("HP")]
    [SerializeField] private Image hpFill;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private RectTransform _hpBarRoot;

    [Header("弾数")]
    [SerializeField] private Transform ammoContainer;      // 弾アイコンを並べる親
    [SerializeField] private Image ammoBulletPrefab;       // 弾アイコンPrefab（Image）
    [SerializeField] private Image reloadCircle;           // リロード中の円形プログレス

    [Header("経験値")]
    [SerializeField] private Image           xpFill;
    [SerializeField] private TextMeshProUGUI levelText;

    private Image[] _ammoIcons;
    private int _lastHP;
    private MotionHandle _xpTween;

    void Start()
    {
        _lastHP = tankStatus.getHP.Value;

        // HP変化を購読（減少時にシェイク演出を追加）
        tankStatus.getHP.Subscribe(hp =>
        {
            if (hp < _lastHP) ShakeHPBar();
            _lastHP = hp;
            RefreshHP(hp, tankStatus.getMaxHP.Value);
        }).AddTo(this);
        tankStatus.getMaxHP.Subscribe(max => RefreshHP(tankStatus.getHP.Value, max)).AddTo(this);

        // 最大弾数変化 → アイコン再構築
        tankStatus.getMagazineCapacity.Subscribe(BuildAmmoIcons).AddTo(this);

        // 現在弾数変化 → アイコン色を更新
        bulletManager.getTotalRounds.Subscribe(RefreshAmmo).AddTo(this);

        if (reloadCircle != null)
            reloadCircle.gameObject.SetActive(false);

        // 経験値変化を購読
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnXpChanged
                .Subscribe(_ => RefreshXp())
                .AddTo(this);
            RefreshXp();
        }

        // Start() の実行順序に依存しないよう 1 フレーム後に強制リフレッシュ
        StartCoroutine(InitAmmoDisplay());
    }

    private IEnumerator InitAmmoDisplay()
    {
        yield return null;
        BuildAmmoIcons(tankStatus.getMagazineCapacity.Value);
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

    private void BuildAmmoIcons(int capacity)
    {
        if (ammoContainer == null || ammoBulletPrefab == null) return;

        foreach (Transform child in ammoContainer)
            if (child != ammoBulletPrefab.transform)
                Destroy(child.gameObject);

        ammoBulletPrefab.gameObject.SetActive(false);

        _ammoIcons = new Image[capacity];
        for (int i = 0; i < capacity; i++)
        {
            var icon = Instantiate(ammoBulletPrefab, ammoContainer);
            icon.gameObject.SetActive(true);
            _ammoIcons[i] = icon;
        }

        RefreshAmmo(bulletManager.getTotalRounds.Value);
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

        int   level    = ExperienceManager.Instance.CurrentLevel;
        int   xp       = ExperienceManager.Instance.CurrentXp;
        int   required = ExperienceManager.Instance.RequiredXp;
        float target   = required > 0 ? (float)xp / required : 0f;

        if (xpFill != null)
        {
            if (_xpTween.IsActive()) _xpTween.Cancel();
            _xpTween = LMotion.Create(xpFill.fillAmount, target, 0.4f)
                .WithEase(Ease.OutCubic)
                .Bind(v => xpFill.fillAmount = v)
                .AddTo(this);
        }

        if (levelText != null)
            levelText.text = $"Lv.{level}";
    }

    private void RefreshAmmo(int rounds)
    {
        if (_ammoIcons == null || theme == null) return;
        for (int i = 0; i < _ammoIcons.Length; i++)
        {
            _ammoIcons[i].color = i < rounds ? theme.textPrimary : theme.border;
        }
    }
}