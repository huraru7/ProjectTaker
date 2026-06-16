using UnityEngine;

public class ChainBullet : MonoBehaviour
{
    private GameObject _prefab;
    private TankStatus _target;
    private TankStatus _owner;
    private int        _damage;
    private int        _remainChain;
    private float      _chainRadius;
    private float      _speed = 12f;

    public void Initialize(GameObject prefab, TankStatus target, TankStatus owner,
                           int damage, int remainChain, float chainRadius)
    {
        _prefab      = prefab;
        _target      = target;
        _owner       = owner;
        _damage      = damage;
        _remainChain = remainChain;
        _chainRadius = chainRadius;
    }

    void Update()
    {
        if (_target == null) { Destroy(gameObject); return; }

        var dir = (_target.transform.position - transform.position).normalized;
        transform.position += dir * _speed * Time.deltaTime;

        if (Vector2.Distance(transform.position, _target.transform.position) < 0.2f)
            Hit();
    }

    private void Hit()
    {
        if (_target == null) { Destroy(gameObject); return; }

        _target.DealDamage(_damage);

        if (_remainChain > 1 && _target.getHP.Value <= 0)
            TryChain(transform.position);

        Destroy(gameObject);
    }

    private void TryChain(Vector2 pos)
    {
        var hits = Physics2D.OverlapCircleAll(pos, _chainRadius);
        TankStatus next = null;
        float minDist = float.MaxValue;

        foreach (var h in hits)
        {
            var s = h.GetComponentInParent<TankStatus>();
            if (s == null || s == _owner || s == _target) continue;
            float d = Vector2.Distance(pos, s.transform.position);
            if (d < minDist) { minDist = d; next = s; }
        }

        if (next == null) return;

        var chain = Instantiate(_prefab, pos, Quaternion.identity);
        chain.GetComponent<ChainBullet>()
            .Initialize(_prefab, next, _owner, _damage, _remainChain - 1, _chainRadius);
    }
}
