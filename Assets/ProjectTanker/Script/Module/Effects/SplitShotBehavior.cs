using UnityEngine;

public class SplitShotBehavior : MonoBehaviour
{
    private GameObject _miniPrefab;
    private int        _miniCount;
    private int        _damage;
    private TankStatus _owner;
    private bool       _hasSplit;

    public void Initialize(GameObject miniPrefab, int miniCount, int damage, TankStatus owner)
    {
        _miniPrefab = miniPrefab;
        _miniCount  = miniCount;
        _damage     = damage;
        _owner      = owner;
        GetComponent<Bullet>().OnWallBounce += OnWallBounce;
    }

    void OnDisable()
    {
        _hasSplit = false;
        var b = GetComponent<Bullet>();
        if (b != null) b.OnWallBounce -= OnWallBounce;
    }

    private void OnWallBounce(Vector2 wallNormal)
    {
        if (_hasSplit) return;
        _hasSplit = true;

        Vector2 spawnPos = transform.position;
        for (int i = 0; i < _miniCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            var dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                  Mathf.Sin(angle * Mathf.Deg2Rad));
            // 壁に向かう方向は法線方向へ反転し、必ず壁から離れる半球に収める
            if (Vector2.Dot(dir, wallNormal) < 0f)
                dir = Vector2.Reflect(dir, wallNormal);
            var mini = Instantiate(_miniPrefab, spawnPos, Quaternion.identity);
            mini.GetComponent<MiniBullet>().Initialize(dir, _damage, _owner);
        }

        GetComponent<Bullet>().ForceReturn();
    }
}
