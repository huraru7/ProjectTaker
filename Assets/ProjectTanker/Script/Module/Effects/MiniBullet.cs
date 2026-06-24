using UnityEngine;

public class MiniBullet : MonoBehaviour
{
    [SerializeField] private float speed    = 8f;
    [SerializeField] private float maxRange = 4f;

    private int        _damage;
    private TankStatus _owner;
    private Vector2    _startPos;

    public void Initialize(Vector2 dir, int damage, TankStatus owner)
    {
        _damage   = damage;
        _owner    = owner;
        _startPos = transform.position;
        GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * speed;
    }

    void Update()
    {
        if (Vector2.Distance(_startPos, transform.position) >= maxRange)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<Wall>(out _)) return;

        var s = col.gameObject.GetComponentInParent<TankStatus>();
        if (s != null && s != _owner)
        {
            s.DealDamage(_damage);
            Destroy(gameObject);
        }
    }
}
