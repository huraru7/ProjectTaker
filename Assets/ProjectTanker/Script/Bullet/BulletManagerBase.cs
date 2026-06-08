using UnityEngine;

public abstract class BulletManagerBase : MonoBehaviour
{
    public abstract void TakeDamage(int damage);
    public abstract void ReturnBullet(Bullet bullet);
    public abstract void Fire();
}
