using UnityEngine;

public class ChainLightningController : MonoBehaviour
{
    private TankBulletManager _bm;
    private GameObject        _chainBulletPrefab;
    private int               _damage;
    private int               _chainCount;
    private float             _chainRadius;
    private TankStatus        _owner;

    public void Setup(GameObject chainBulletPrefab, int damage, int chainCount, float chainRadius, TankStatus owner)
    {
        _chainBulletPrefab = chainBulletPrefab;
        _damage      = damage;
        _chainCount  = chainCount;
        _chainRadius = chainRadius;
        _owner       = owner;

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
        var existing = bullet.GetComponent<ChainLightningBehavior>();
        if (existing != null) Destroy(existing);
        bullet.gameObject.AddComponent<ChainLightningBehavior>()
            .Initialize(_chainBulletPrefab, _damage, _chainCount, _chainRadius, _owner);
    }
}
