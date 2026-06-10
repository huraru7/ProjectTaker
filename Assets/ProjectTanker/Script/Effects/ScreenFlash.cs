using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 爆発時などに画面全体を一瞬白くフラッシュさせる演出。
/// Canvas 内の全画面 Image を参照してアルファをアニメーションする。
/// </summary>
public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    [SerializeField] private Image _flashImage;

    void Awake()
    {
        Instance = this;
        if (_flashImage != null)
        {
            var c = _flashImage.color;
            c.a = 0f;
            _flashImage.color = c;
        }
    }

    /// <summary>アルファ 0.6 → 0 を 0.1 秒で消す白フラッシュを再生する。</summary>
    public void Flash()
    {
        if (_flashImage == null) return;

        var c = _flashImage.color;
        c.a = 0.6f;
        _flashImage.color = c;

        LMotion.Create(0.6f, 0f, 0.1f)
            .WithEase(Ease.OutQuad)
            .BindToColorA(_flashImage)
            .AddTo(this);
    }
}
