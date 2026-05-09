using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    //do:弾同士の衝突の挙動をどうするか
    [Tooltip("BulletSetting")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 5f;
    private float speed;
    private float _currentLifeTime;
    private Rigidbody2D _rb;
    private Vector2 _direction;
    private TankBulletManager _owner;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 弾を初期化し、指定した方向へ発射する。
    /// </summary>
    /// <param name="direction">発射方向</param>
    /// <param name="owner">この弾を発射したタンクのBulletManager</param>
    /// <param name="speed">弾速</param>
    public void Initialize(Vector2 direction, TankBulletManager owner, float speed)
    {
        this.speed = speed;
        _owner = owner;
        _direction = direction.normalized;
        _rb.linearVelocity = _direction * speed;
        _currentLifeTime = lifeTime;
    }

    void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0f && _owner != null)
        {
            _owner.ReturnBullet(this);
        }
    }

    void OnDisable()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out _))
        {
            Vector2 normal = collision.contacts[0].normal;
            _direction = Vector2.Reflect(_direction, normal);
            _rb.linearVelocity = _direction * speed;
            return;
        }

        if (collision.gameObject.TryGetComponent<TankBulletManager>(out var bulletManager))
        {
            bulletManager.TakeDamage(damage);
            if (_owner != null) _owner.ReturnBullet(this);
        }
    }
}
