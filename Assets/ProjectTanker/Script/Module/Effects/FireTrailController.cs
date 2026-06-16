using UnityEngine;

public class FireTrailController : MonoBehaviour
{
    private TankBulletManager _bm;
    private GameObject        _prefab;
    private float             _interval;
    private float             _duration;
    private int               _damage;
    private TankStatus        _owner;

    public void Setup(GameObject prefab, float interval, float duration, int damage, TankStatus owner)
    {
        _prefab   = prefab;
        _interval = interval;
        _duration = duration;
        _damage   = damage;
        _owner    = owner;

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
        var existing = bullet.GetComponent<FireTrailBehavior>();
        if (existing != null) Destroy(existing);
        bullet.gameObject.AddComponent<FireTrailBehavior>()
            .Initialize(_prefab, _interval, _duration, _damage, _owner);
    }
}
