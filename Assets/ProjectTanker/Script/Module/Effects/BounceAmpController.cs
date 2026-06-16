using UnityEngine;

public class BounceAmpController : MonoBehaviour
{
    private TankBulletManager _bm;
    private int               _bonusPerBounce;
    private TankStatus        _owner;

    public void Setup(int bonusPerBounce)
    {
        _bonusPerBounce = bonusPerBounce;

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
        var existing = bullet.GetComponent<BounceAmpBehavior>();
        if (existing != null) Destroy(existing);
        bullet.gameObject.AddComponent<BounceAmpBehavior>()
            .Initialize(_bonusPerBounce);
    }
}
