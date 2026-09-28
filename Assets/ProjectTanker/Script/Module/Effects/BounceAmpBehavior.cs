using UnityEngine;

public class BounceAmpBehavior : MonoBehaviour
{
    private Bullet         _bullet;
    private SpriteRenderer _sr;
    private int            _bonusPerBounce;
    private int            _bounceCount;
    private bool           _active;

    private static readonly Color[] _bounceColors =
    {
        Color.white,
        new Color(1f, 0.71f, 0f),
        new Color(1f, 0.31f, 0f),
        new Color(1f, 0.1f,  0f),
    };

    public void Initialize(int bonusPerBounce)
    {
        if (_bullet != null) _bullet.OnWallBounce -= OnBounce; // 再初期化時の二重購読を防ぐ
        _bullet         = GetComponent<Bullet>();
        _sr             = GetComponent<SpriteRenderer>();
        _bonusPerBounce = bonusPerBounce;
        _bounceCount    = 0;
        _active         = true;
        _bullet.OnWallBounce += OnBounce;
    }

    void OnDisable()
    {
        _active = false;
        if (_bullet != null) _bullet.OnWallBounce -= OnBounce;
        if (_bullet != null) _bullet.BonusDamage = 0;
        if (_sr    != null) _sr.color = Color.white;
        _bounceCount = 0;
    }

    private void OnBounce(Vector2 _)
    {
        if (!_active) return;
        _bounceCount++;
        _bullet.BonusDamage += _bonusPerBounce;

        int colorIdx = Mathf.Min(_bounceCount, _bounceColors.Length - 1);
        if (_sr != null) _sr.color = _bounceColors[colorIdx];
    }
}
