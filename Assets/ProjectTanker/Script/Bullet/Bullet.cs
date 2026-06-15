using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class Bullet : MonoBehaviour
{
    [Tooltip("BulletSetting")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 5f;

    [Header("Trail")]
    [SerializeField] private int _trailLength = 12;

    private float speed;
    private float _currentLifeTime;
    private Rigidbody2D _rb;
    private Vector2 _direction;
    private BulletManagerBase _owner;

    private LineRenderer _lineRenderer;
    private Vector3[] _trailPositions;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _trailPositions = new Vector3[_trailLength];
        SetupTrail();
    }

    private void SetupTrail()
    {
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _lineRenderer.receiveShadows = false;
        _lineRenderer.positionCount = 0;
        _lineRenderer.sortingOrder = 10;

        // 前端（現在位置）が太く後端（軌跡末尾）が細くなる幅カーブ
        _lineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0f, 0.08f),
            new Keyframe(1f, 0.02f));

        // #F5C518 黄色：前端が不透明、後端が透明
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.961f, 0.773f, 0.094f), 0f),
                new GradientColorKey(new Color(0.961f, 0.773f, 0.094f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.8f, 0f),
                new GradientAlphaKey(0f,   1f)
            });
        _lineRenderer.colorGradient = gradient;
        _lineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
    }

    /// <summary>弾を初期化し、指定した方向へ発射する。</summary>
    public void Initialize(Vector2 direction, BulletManagerBase owner, float speed)
    {
        this.speed = speed;
        _owner = owner;
        _direction = direction.normalized;
        _rb.linearVelocity = _direction * speed;
        _currentLifeTime = lifeTime;

        // トレイル位置を現在位置でリセット
        for (int i = 0; i < _trailLength; i++)
            _trailPositions[i] = transform.position;
        _lineRenderer.positionCount = _trailLength;
        _lineRenderer.SetPositions(_trailPositions);
    }

    void Update()
    {
        // トレイル位置を 1 つずつ後ろへシフトして先頭を現在位置に更新
        for (int i = _trailLength - 1; i > 0; i--)
            _trailPositions[i] = _trailPositions[i - 1];
        _trailPositions[0] = transform.position;
        _lineRenderer.SetPositions(_trailPositions);

        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0f && _owner != null)
            _owner.ReturnBullet(this);
    }

    void OnDisable()
    {
        _rb.linearVelocity = Vector2.zero;
        // プールへ返却時にトレイルを非表示にする
        _lineRenderer.positionCount = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out _))
        {
            // 壁被弾跡を生成
            var contact = collision.contacts[0];
            BulletMarkSpawner.Instance?.Spawn(contact.point, contact.normal);

            Vector2 normal = contact.normal;
            _direction = Vector2.Reflect(_direction, normal);
            _rb.linearVelocity = _direction * speed;
            return;
        }

        if (collision.gameObject.TryGetComponent<BulletManagerBase>(out var bulletManager))
        {
            bulletManager.TakeDamage(damage);
            if (_owner != null) _owner.ReturnBullet(this);
        }
    }
}
