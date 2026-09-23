using UnityEngine;

public class StageStatTracker : MonoBehaviour
{
    [SerializeField] private TankBulletManager _bulletManager;
    [SerializeField] private Transform         _playerTransform;

    public int   ShotCount      { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public float TraveledMeters { get; private set; }

    private float   _startTime;
    private Vector3 _prevPosition;
    private bool    _tracking;

    void Start()
    {
        _startTime    = Time.time;
        _prevPosition = _playerTransform.position;
        _tracking     = true;
        _bulletManager.OnBulletFired += OnBulletFired;
    }

    void OnDestroy()
    {
        if (_bulletManager != null)
            _bulletManager.OnBulletFired -= OnBulletFired;
    }

    void FixedUpdate()
    {
        if (!_tracking || _playerTransform == null) return;
        TraveledMeters += Vector3.Distance(_playerTransform.position, _prevPosition);
        _prevPosition   = _playerTransform.position;
    }

    public void StopTracking()
    {
        _tracking       = false;
        ElapsedSeconds  = Time.time - _startTime;
        _bulletManager.OnBulletFired -= OnBulletFired;
    }

    /// <summary>
    /// 次のステージ開始時に計測をリセットして再開する。
    /// </summary>
    public void ResetAndStart()
    {
        ShotCount      = 0;
        TraveledMeters = 0f;
        ElapsedSeconds = 0f;
        _startTime     = Time.time;
        _prevPosition  = _playerTransform.position;
        _tracking      = true;
        _bulletManager.OnBulletFired += OnBulletFired;
    }

    private void OnBulletFired(Bullet _) => ShotCount++;
}
