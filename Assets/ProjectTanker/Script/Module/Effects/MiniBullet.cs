using UnityEngine;

public class MiniBullet : MonoBehaviour
{
    [SerializeField] private float speed          = 8f;
    [SerializeField] private float maxRange        = 4f;
    [SerializeField] private float wallIgnoreTime  = 0.1f;

    private int        _damage;
    private TankStatus _owner;
    private Vector2    _startPos;
    private float      _elapsed;

    public void Initialize(Vector2 dir, int damage, TankStatus owner)
    {
        _damage   = damage;
        _owner    = owner;
        _startPos = transform.position;
        _elapsed  = 0f;
        GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * speed;
    }

    void Update()
    {
        _elapsed += Time.deltaTime;

        if (Vector2.Distance(_startPos, transform.position) >= maxRange)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // 分裂直後は元弾の被弾地点（壁の近傍）に生成されるため、生成直後だけ壁との接触を無視して脱出させる
        if (_elapsed < wallIgnoreTime && col.gameObject.TryGetComponent<Wall>(out _)) return;

        var s = col.gameObject.GetComponentInParent<TankStatus>();
        if (s != null && s != _owner) s.DealDamage(_damage);
        Destroy(gameObject);
    }
}
