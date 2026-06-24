using UnityEngine;

public class SplitShotController : MonoBehaviour
{
    private TankBulletManager _bm;
    private GameObject        _explosionPrefab;
    private float             _radius;
    private int               _damage;
    private TankStatus        _owner;

    public void Setup(GameObject explosionPrefab, float radius, int damage, TankStatus owner)
    {
        _explosionPrefab = explosionPrefab;
        _radius = radius;
        _damage = damage;
        _owner  = owner;

        if (_bm == null)
        {
            _bm = GetComponent<TankBulletManager>();
            if (_bm != null) _bm.OnBulletFired += HandleBulletFired;
        }
    }

    void OnDestroy()
    {
        if (_bm != null) _bm.OnBulletFired -= HandleBulletFired;
    }

    private void HandleBulletFired(Bullet bullet)
    {
        var existing = bullet.GetComponent<SplitShotBehavior>();
        if (existing != null) Destroy(existing);
        bullet.gameObject.AddComponent<SplitShotBehavior>()
            .Initialize(_explosionPrefab, _radius, _damage, _owner);
    }
}
