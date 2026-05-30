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

    [Header("弾数")]
    [SerializeField] private Transform ammoContainer;      // 弾アイコンを並べる親
    [SerializeField] private Image ammoBulletPrefab;       // 弾アイコンPrefab（Image）
    [SerializeField] private Image reloadCircle;           // リロード中の円形プログレス

    private Image[] _ammoIcons;

    void Start()
    {
        // HP変化を購読
        tankStatus.getHP.Subscribe(hp => RefreshHP(hp, tankStatus.getMaxHP.Value)).AddTo(this);
        tankStatus.getMaxHP.Subscribe(max => RefreshHP(tankStatus.getHP.Value, max)).AddTo(this);

        // 最大弾数変化 → アイコン再構築
        tankStatus.getMagazineCapacity.Subscribe(BuildAmmoIcons).AddTo(this);

        // 現在弾数変化 → アイコン色を更新
        bulletManager.getTotalRounds.Subscribe(RefreshAmmo).AddTo(this);

        if (reloadCircle != null)
            reloadCircle.gameObject.SetActive(false);
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

        // HP残量に応じて fire(赤) → wind(黄) でグラデーション
        if (theme != null)
            hpFill.color = Color.Lerp(theme.fire, theme.wind, ratio);

        if (hpText != null)
            hpText.text = $"{hp} / {max}";
    }

    private void BuildAmmoIcons(int capacity)
    {
        if (ammoContainer == null || ammoBulletPrefab == null) return;

        foreach (Transform child in ammoContainer)
            Destroy(child.gameObject);

        _ammoIcons = new Image[capacity];
        for (int i = 0; i < capacity; i++)
        {
            var icon = Instantiate(ammoBulletPrefab, ammoContainer);
            icon.gameObject.SetActive(true);
            _ammoIcons[i] = icon;
        }

        RefreshAmmo(bulletManager.getTotalRounds.Value);
    }

    private void RefreshAmmo(int rounds)
    {
        if (_ammoIcons == null || theme == null) return;
        for (int i = 0; i < _ammoIcons.Length; i++)
        {
            // 残弾のアイコンは濃色、消費済みは薄色
            _ammoIcons[i].color = i < rounds ? theme.textPrimary : theme.border;
        }
    }
}
