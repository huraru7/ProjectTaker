using LitMotion;
using UnityEngine;

/// <summary>
/// 汎用の範囲爆発エフェクト。範囲ダメージ＋リング拡大アニメを実行して自動削除。
/// SplitShot（反射爆発）・爆弾など複数の機能から共通して利用する。
/// </summary>
public class AreaExplosion : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MaterialPropertyBlock _mpb;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _mpb = new MaterialPropertyBlock();
    }

    /// <summary>爆発を実行する。呼び出し直後に範囲ダメージを与え、リングアニメ後に自動削除される。</summary>
    public void Initialize(float radius, int damage, TankStatus owner)
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var h in hits)
        {
            var status = h.GetComponentInParent<TankStatus>();
            if (status != null && status != owner)
                status.DealDamage(damage);
        }

        CameraShake.Instance?.Shake(0.35f, 0.2f);
        HitStop.Instance?.Execute(0.06f);

        float targetScale = radius * 2f;
        LMotion.Create(0f, 1f, 0.4f)
            .WithEase(Ease.OutQuart)
            .WithOnComplete(() => Destroy(gameObject))
            .Bind(t =>
            {
                transform.localScale = Vector3.one * (targetScale * t);
                _mpb.SetFloat("_Alpha", 1f - Mathf.Pow(t, 0.5f));
                _renderer.SetPropertyBlock(_mpb);
            })
            .AddTo(this);
    }
}
