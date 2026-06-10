using UnityEngine;

/// <summary>
/// 爆発エフェクト。Explosion.prefab に付ける MonoBehaviour。
/// Flash / Fireball / Debris の 3 つの ParticleSystem を子として参照する。
/// SpawnAt() で任意の位置に生成して自動的に演出を再生・削除する。
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem _flash;
    [SerializeField] private ParticleSystem _fireball;
    [SerializeField] private ParticleSystem _debris;

    /// <summary>
    /// Inspector でこのフィールドに Explosion.prefab をアサインしておく。
    /// タンク死亡時に ExplosionEffect.SpawnAt(position) を呼べばよい。
    /// </summary>
    public static ExplosionEffect Prefab;

    /// <summary>指定ワールド座標に爆発エフェクトを生成して再生する。</summary>
    public static void SpawnAt(Vector2 position)
    {
        if (Prefab == null)
        {
            Debug.LogWarning("[ExplosionEffect] Prefab が設定されていません。ExplosionEffectSpawner を Scene に配置してください。");
            return;
        }
        var fx = Instantiate(Prefab, position, Quaternion.identity);
        fx.Play();
    }

    /// <summary>パーティクル再生 + 演出トリガー + 自動削除を開始する。</summary>
    public void Play()
    {
        _flash?.Play();
        _fireball?.Play();
        _debris?.Play();

        ScreenFlash.Instance?.Flash();
        HitStop.Instance?.Execute(0.08f);
        CameraShake.Instance?.Shake(0.3f, 0.25f);

        float totalDuration = Mathf.Max(
            _fireball != null ? _fireball.main.duration + _fireball.main.startLifetime.constantMax : 0.7f,
            _debris   != null ? _debris.main.duration   + _debris.main.startLifetime.constantMax   : 1.1f
        );
        Destroy(gameObject, totalDuration + 0.2f);
    }
}
