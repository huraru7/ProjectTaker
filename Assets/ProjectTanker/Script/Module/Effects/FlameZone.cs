using UnityEngine;

public class FlameZone : MonoBehaviour
{
    private int        _damage;
    private TankStatus _owner;

    public void Initialize(float duration, int damage, TankStatus owner)
    {
        _damage = damage;
        _owner  = owner;
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var s = other.GetComponentInParent<TankStatus>();
        if (s == null || s == _owner) return;
        s.DealDamage(_damage);
    }
}
