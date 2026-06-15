using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ExpOrb : MonoBehaviour
{
    [SerializeField] private float attractRadius = 4f;
    [SerializeField] private float moveSpeed     = 7f;

    private int         _xpValue;
    private Transform   _player;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearDamping = 4f; // 初速をすぐ減衰させる
    }

    public void Initialize(int xpValue, Transform player, Vector2 initialVelocity)
    {
        _xpValue           = xpValue;
        _player            = player;
        _rb.linearVelocity = initialVelocity;
    }

    void FixedUpdate()
    {
        if (_player == null) return;
        float dist = Vector2.Distance(transform.position, _player.position);
        if (dist < attractRadius)
        {
            Vector2 dir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, dir * moveSpeed, Time.fixedDeltaTime * 10f);
        }
    }

    // IsTrigger=true の CircleCollider2D がプレイヤーに重なったとき呼ばれる
    void OnTriggerEnter2D(Collider2D other)
    {
        // GetComponentInParent でコライダーが子オブジェクトにある場合も検出する
        if (other.GetComponentInParent<TankMovement>() == null) return;
        ExperienceManager.Instance.CollectXp(_xpValue);
        ExperienceManager.Instance.ReturnOrb(this);
    }

    void OnDisable() => _rb.linearVelocity = Vector2.zero;
}
