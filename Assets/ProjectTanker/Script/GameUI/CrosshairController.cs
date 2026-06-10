using LitMotion;
using LitMotion.Extensions;
using R3;
using UnityEngine;

/// <summary>
/// マウス位置に追従する照準レティクル。
/// 発射時（弾数が減少した瞬間）に LitMotion でスケールパルスを再生する。
/// </summary>
public class CrosshairController : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private RectTransform _crosshairRoot;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TankBulletManager _bulletManager;

    void Start()
    {
        Cursor.visible = false;

        if (_bulletManager == null) return;

        // 弾数が減少した瞬間 = 発射 → パルスアニメーション
        _bulletManager.getTotalRounds
            .Pairwise()
            .Where(p => p.Current < p.Previous)
            .Subscribe(_ => PulseCrosshair())
            .AddTo(this);
    }

    void Update()
    {
        if (_crosshairRoot == null || _canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            Input.mousePosition,
            _canvas.worldCamera,
            out Vector2 localPoint);

        _crosshairRoot.anchoredPosition = localPoint;
    }

    private void PulseCrosshair()
    {
        if (_crosshairRoot == null) return;

        // Scale 1.0 → 1.3 → 1.0 (0.1 秒)
        LMotion.Punch.Create(Vector3.one, Vector3.one * 0.3f, 0.1f)
            .WithFrequency(1)
            .WithDampingRatio(1f)
            .BindToLocalScale(_crosshairRoot)
            .AddTo(this);
    }

    void OnDestroy() => Cursor.visible = true;
}
