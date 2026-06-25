using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankBulletManager : BulletManagerBase
{
    [Header("TankStatus")]
    [SerializeField] private TankStatus _tankStatus;

    [Header("Barrel")]
    [SerializeField] private BarrelController _barrel;

    [Header("予告線")]
    [SerializeField] private LineRenderer _aimLine;
    [SerializeField] private float        _aimLineLength = 8f;
    [SerializeField] private int          _maxBounces    = 2;
    private Material  _aimLineMat;
    private Texture2D _dotTexture;

    [Header("Setting")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private bool isShoot = true;
    [SerializeField] private SerializableReactiveProperty<int> totalRounds;
    public SerializableReactiveProperty<int> getTotalRounds => totalRounds;

    public event System.Action<Bullet> OnBulletFired;

    public float ReloadProgress => _tankStatus != null && totalRounds.Value < _tankStatus.getMagazineCapacity.Value
        ? Mathf.Clamp01(currentTime / _tankStatus.getReloadTime.Value)
        : 0f;

    [Header("PoolSize")]
    [SerializeField] private int _poolSize = 5;
    [SerializeField] private Transform _bulletParent;
    private Queue<Bullet> _pool = new Queue<Bullet>();

    void Awake()
    {
        _wallMask = LayerMask.GetMask("Wall");
        totalRounds.Value = 0;
        for (int i = 0; i < _poolSize; i++)
        {
            var obj = Instantiate(bullet, transform.position, Quaternion.identity, _bulletParent);
            obj.SetActive(false);
            _pool.Enqueue(obj.GetComponent<Bullet>());
        }
        SetupAimLine();
    }

    private void SetupAimLine()
    {
        if (_aimLine == null) return;
        _aimLine.useWorldSpace   = true;
        _aimLine.positionCount   = 2;
        _aimLine.widthMultiplier = 0.04f;
        _aimLine.textureMode     = LineTextureMode.Tile;
        _aimLine.sortingOrder    = 5;

        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new(new Color(0.2f, 0.9f, 0.3f), 0f), new(new Color(0.2f, 0.9f, 0.3f), 1f) },
            new GradientAlphaKey[] { new(0.6f, 0f), new(0.6f, 1f) });
        _aimLine.colorGradient = grad;

        _dotTexture            = new Texture2D(16, 4, TextureFormat.RGBA32, false);
        _dotTexture.wrapMode   = TextureWrapMode.Repeat;
        _dotTexture.filterMode = FilterMode.Point;
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 16; x++)
                _dotTexture.SetPixel(x, y, x < 8 ? Color.white : Color.clear);
        _dotTexture.Apply();

        _aimLineMat                  = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        _aimLineMat.mainTexture      = _dotTexture;
        _aimLineMat.mainTextureScale = new Vector2(4f, 1f);

        // アルファ透過を有効化（デフォルトは Opaque → 透明部が黒になる）
        _aimLineMat.SetFloat("_Surface", 1f);
        _aimLineMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _aimLineMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _aimLineMat.SetInt("_ZWrite", 0);
        _aimLineMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        _aimLine.material            = _aimLineMat;
    }

    void OnDestroy()
    {
        if (_aimLineMat  != null) Destroy(_aimLineMat);
        if (_dotTexture  != null) Destroy(_dotTexture);
    }

    void Start()
    {
        totalRounds.Value = _tankStatus.getMagazineCapacity.Value;
        Debug.Log($"初期弾数: {totalRounds.Value}");
    }

    public override void TakeDamage(int damage)
    {
        _tankStatus.DealDamage(damage);
    }

    private float currentTime = 0f;

    void Update()
    {
        if (totalRounds.Value < _tankStatus.getMagazineCapacity.Value)
        {
            currentTime += Time.deltaTime;
            if (currentTime > _tankStatus.getReloadTime.Value)
            {
                totalRounds.Value++;
                currentTime = 0f;
                Debug.Log($"りろーど 弾残量{totalRounds.Value}");
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Fire();
            Debug.Log($"弾残量{totalRounds.Value}");
        }

        UpdateAimLine();
    }

    private int _wallMask;

    private void UpdateAimLine()
    {
        if (_aimLine == null) return;

        var     points    = new List<Vector3>();
        Vector2 origin    = _barrel.MuzzlePosition;
        Vector2 dir       = _barrel.AimDirection;
        float   remaining = _aimLineLength;

        points.Add(origin);

        for (int bounce = 0; bounce <= _maxBounces; bounce++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, remaining, _wallMask);

            if (hit)
            {
                points.Add(hit.point);
                remaining -= hit.distance;
                dir    = Vector2.Reflect(dir, hit.normal);
                origin = hit.point + hit.normal * 0.05f;
            }
            else
            {
                points.Add(origin + dir * remaining);
                break;
            }
        }

        _aimLine.positionCount = points.Count;
        _aimLine.SetPositions(points.ToArray());
    }

    public void SpawnBullet(Vector2 direction)
    {
        Bullet b;
        if (_pool.Count > 0)
        {
            b = _pool.Dequeue();
        }
        else
        {
            var obj = Instantiate(bullet, _barrel.MuzzlePosition, Quaternion.identity, _bulletParent);
            b = obj.GetComponent<Bullet>();
        }

        b.transform.position = _barrel.MuzzlePosition;
        b.gameObject.SetActive(true);
        b.Initialize(direction, this, _tankStatus.getBulletSpeed.Value);
        totalRounds.Value--;
        OnBulletFired?.Invoke(b);
        AudioManager.Instance?.PlayShoot();
    }

    public override void Fire()
    {
        if (isShoot && totalRounds.Value >= 1)
            SpawnBullet(_barrel.AimDirection);
    }

    public override void ReturnBullet(Bullet b)
    {
        b.gameObject.SetActive(false);
        _pool.Enqueue(b);
    }
}
