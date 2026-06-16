using LitMotion;
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
        transform.localScale = Vector3.one;
        _bounceCount = 0;
    }

    private void OnBounce()
    {
        if (!_active) return;
        _bounceCount++;
        _bullet.BonusDamage += _bonusPerBounce;

        int colorIdx = Mathf.Min(_bounceCount, _bounceColors.Length - 1);
        if (_sr != null) _sr.color = _bounceColors[colorIdx];

        float targetScale = 1f + 0.15f * Mathf.Min(_bounceCount, 3);
        LMotion.Create(targetScale + 0.2f, targetScale, 0.15f)
            .WithEase(Ease.OutBack)
            .Bind(s => { if (this != null) transform.localScale = Vector3.one * s; })
            .AddTo(this);
    }
}
