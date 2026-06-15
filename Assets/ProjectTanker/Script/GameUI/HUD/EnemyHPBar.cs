using R3;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _fill;
    [SerializeField] private TankStatus     _status;

    private float _maxScaleX;

    void Start()
    {
        _maxScaleX = _fill.transform.localScale.x;
        _status.getHP.Subscribe(hp => Refresh(hp, _status.getMaxHP.Value)).AddTo(this);
        _status.getMaxHP.Subscribe(max => Refresh(_status.getHP.Value, max)).AddTo(this);
    }

    private void Refresh(int hp, int max)
    {
        if (max <= 0) return;
        var s = _fill.transform.localScale;
        _fill.transform.localScale = new Vector3(_maxScaleX * (float)hp / max, s.y, s.z);
    }
}