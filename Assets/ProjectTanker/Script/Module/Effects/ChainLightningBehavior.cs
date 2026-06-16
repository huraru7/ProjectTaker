using UnityEngine;

public class ChainLightningBehavior : MonoBehaviour
{
    private GameObject _chainBulletPrefab;
    private int        _damage;
    private int        _chainCount;
    private float      _chainRadius;
    private TankStatus _owner;
    private bool       _active;

    public void Initialize(GameObject chainBulletPrefab, int damage,
                           int chainCount, float chainRadius, TankStatus owner)
    {
        _chainBulletPrefab = chainBulletPrefab;
        _damage      = damage;
        _chainCount  = chainCount;
        _chainRadius = chainRadius;
        _owner       = owner;
        _active      = true;
        GetComponent<Bullet>().OnImpact += OnImpact;
    }

    void OnDisable()
    {
        _active = false;
        var b = GetComponent<Bullet>();
        if (b != null) b.OnImpact -= OnImpact;
    }

    private void OnImpact(Vector2 pos, TankStatus target)
    {
        if (!_active || target == null) return;
        if (target.getHP.Value > 0) return;

        var hits = Physics2D.OverlapCircleAll(pos, _chainRadius);
        TankStatus next = null;
        float minDist = float.MaxValue;

        foreach (var h in hits)
        {
            var s = h.GetComponentInParent<TankStatus>();
            if (s == null || s == _owner || s == target) continue;
            float d = Vector2.Distance(pos, s.transform.position);
            if (d < minDist) { minDist = d; next = s; }
        }

        if (next == null) return;

        var chain = Instantiate(_chainBulletPrefab, pos, Quaternion.identity);
        chain.GetComponent<ChainBullet>()
            .Initialize(_chainBulletPrefab, next, _owner, _damage, _chainCount, _chainRadius);
    }
}
