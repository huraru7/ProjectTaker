using UnityEngine;

public class SplitShotBehavior : MonoBehaviour
{
    private GameObject _explosionPrefab;
    private float      _radius;
    private int        _damage;
    private TankStatus _owner;

    public void Initialize(GameObject explosionPrefab, float radius, int damage, TankStatus owner)
    {
        _explosionPrefab = explosionPrefab;
        _radius  = radius;
        _damage  = damage;
        _owner   = owner;
        GetComponent<Bullet>().OnWallBounce += OnWallBounce;
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
