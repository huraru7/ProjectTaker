using System.Collections.Generic;
using R3;
using UnityEngine;

public class EnemyBulletManager : BulletManagerBase
{
    [Header("TankStatus")]
    [SerializeField] private TankStatus _tankStatus;

    [Header("Setting")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private bool isShoot = true;
    [SerializeField] private SerializableReactiveProperty<int> totalRounds;
    public SerializableReactiveProperty<int> getTotalRounds => totalRounds;

    public float ReloadProgress => _tankStatus != null && totalRounds.Value < _tankStatus.getMagazineCapacity.Value
        ? Mathf.Clamp01(currentTime / _tankStatus.getReloadTime.Value)
        : 0f;

    [Header("PoolSize")]
    [SerializeField] private int _poolSize = 5;
    [SerializeField] private Transform _bulletParent;
    private Queue<Bullet> _pool = new Queue<Bullet>();

    void Awake()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            var obj = Instantiate(bullet, transform.position, Quaternion.identity, _bulletParent);
            obj.SetActive(false);
            _pool.Enqueue(obj.GetComponent<Bullet>());
        }
    }

    void Start()
    {
        totalRounds.Value = _tankStatus.getMagazineCapacity.Value;
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
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        _tankStatus.DealDamage(damage);
    }

    public override void Fire()
    {
        if (isShoot && totalRounds.Value >= 1)
            SpawnBullet(transform.up);
    }

    public void FireInDirection(Vector2 direction)
    {
        if (isShoot && totalRounds.Value >= 1)
            SpawnBullet(direction);
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
            var obj = Instantiate(bullet, transform.position, Quaternion.identity, _bulletParent);
            b = obj.GetComponent<Bullet>();
        }

        b.transform.position = transform.position;
        b.gameObject.SetActive(true);
        b.Initialize(direction, this, _tankStatus.getBulletSpeed.Value);
        totalRounds.Value--;
    }

    public override void ReturnBullet(Bullet b)
    {
        b.gameObject.SetActive(false);
        _pool.Enqueue(b);
    }
}
