using UnityEngine;

public class SplitShotBehavior : MonoBehaviour
{
    private GameObject _explosionPrefab;
    private float      _radius;
    private int        _damage;
    private TankStatus _owner;

    private Bullet _bullet;

    public void Initialize(GameObject explosionPrefab, float radius, int damage, TankStatus owner)
    {
        if (_bullet != null) _bullet.OnWallBounce -= OnWallBounce; // 再初期化時の二重購読を防ぐ
        _explosionPrefab = explosionPrefab;
        _radius  = radius;
        _damage  = damage;
        _owner   = owner;
        _bullet  = GetComponent<Bullet>();
        _bullet.OnWallBounce += OnWallBounce;
    }

    void OnDisable()
    {
        var b = GetComponent<Bullet>();
        if (b != null) b.OnWallBounce -= OnWallBounce;
    }

    private void OnWallBounce(Vector2 _)
    {
        var go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        go.GetComponent<AreaExplosion>().Initialize(_radius, _damage, _owner);
    }
}
