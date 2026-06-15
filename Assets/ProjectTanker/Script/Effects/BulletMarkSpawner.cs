using UnityEngine;

/// <summary>
/// BulletMark を生成するシングルトン。
/// Bullet.cs からの壁衝突コールバックで呼ばれる。
/// </summary>
public class BulletMarkSpawner : MonoBehaviour
{
    public static BulletMarkSpawner Instance { get; private set; }

    [SerializeField] private BulletMark _markPrefab;

    void Awake() => Instance = this;

    /// <summary>指定位置に被弾跡を生成する。</summary>
    public void Spawn(Vector2 position, Vector2 normal)
    {
        if (_markPrefab == null) return;
        var mark = Instantiate(_markPrefab, position, Quaternion.identity);
        mark.Place(position, normal);
    }
}
