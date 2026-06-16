using UnityEngine;

public class SplitShotController : MonoBehaviour
{
    private TankBulletManager _bm;
    private GameObject        _miniPrefab;
    private int               _miniCount;
    private int               _damage;
    private TankStatus        _owner;

    public void Setup(GameObject miniPrefab, int miniCount, int damage, TankStatus owner)
    {
        _miniPrefab = miniPrefab;
        _miniCount  = miniCount;
        _damage     = damage;
        _owner      = owner;

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
            .Initialize(_miniPrefab, _miniCount, _damage, _owner);
    }
}
