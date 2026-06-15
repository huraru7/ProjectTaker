using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

/// <summary>
/// 弾が壁に当たった跡として生成される小さな円マーク。
/// fadeDelay 秒後に fadeDuration 秒かけてアルファ 0 にフェードし自動削除される。
/// </summary>
public class BulletMark : MonoBehaviour
{
    [SerializeField] private float _fadeDelay = 8f;
    [SerializeField] private float _fadeDuration = 2f;

    private SpriteRenderer _sr;

    void Awake() => _sr = GetComponent<SpriteRenderer>();

    /// <summary>指定位置・法線方向に被弾跡を配置しフェードアニメーションを開始する。</summary>
    public void Place(Vector2 position, Vector2 normal)
    {
        // 壁サーフェスからわずかに浮かせて Z ファイトを防ぐ
        transform.position = (Vector3)(position + normal * 0.02f);

        // ランダム回転で自然な見た目に
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        // アルファを初期値に設定
        if (_sr != null)
        {
            var c = _sr.color;
            c.a = 0.4f;
            _sr.color = c;
        }

        LMotion.Create(0.4f, 0f, _fadeDuration)
            .WithDelay(_fadeDelay)
            .WithOnComplete(() => Destroy(gameObject))
            .BindToColorA(_sr)
            .AddTo(this);
    }
}
