using UnityEngine;

public class MainCameraFollower : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _TankTarget;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -10);
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("追従軸")]
    [SerializeField] private bool _followX = true;
    [SerializeField] private bool _followY = true;
    [SerializeField] private bool _followZ = false;

    [Header("カメラ移動制限")]
    [Tooltip("PolygonCollider2D を持つ GameObject をアサイン")]
    [SerializeField] private PolygonCollider2D _cameraBounds;

    private float _halfHeight;
    private float _halfWidth;
    private Vector3 _smoothedPosition;

    void Start()
    {
        _smoothedPosition = transform.position;
        RecalcCameraSize();
    }

    void OnValidate()
    {
        RecalcCameraSize();
    }

    void RecalcCameraSize()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null) return;
        _halfHeight = cam.orthographicSize;
        _halfWidth = _halfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (_TankTarget == null) return;

        Vector3 targetPosition = _TankTarget.position + _offset;

        Vector3 newPosition = new Vector3(
            _followX ? targetPosition.x : _smoothedPosition.x,
            _followY ? targetPosition.y : _smoothedPosition.y,
            _followZ ? targetPosition.z : _smoothedPosition.z
        );

        if (_cameraBounds != null)
            newPosition = ClampToBounds(newPosition);

        _smoothedPosition = Vector3.Lerp(_smoothedPosition, newPosition, _smoothSpeed * Time.deltaTime);

        Vector3 shakeOffset = CameraShake.Instance != null ? CameraShake.Instance.ShakeOffset : Vector3.zero;
        transform.position = _smoothedPosition + shakeOffset;
    }

    Vector3 ClampToBounds(Vector3 desiredPos)
    {
        Bounds b = _cameraBounds.bounds;

        float minX = b.min.x + _halfWidth;
        float maxX = b.max.x - _halfWidth;
        float minY = b.min.y + _halfHeight;
        float maxY = b.max.y - _halfHeight;

        float clampedX = (minX < maxX) ? Mathf.Clamp(desiredPos.x, minX, maxX) : b.center.x;
        float clampedY = (minY < maxY) ? Mathf.Clamp(desiredPos.y, minY, maxY) : b.center.y;

        return new Vector3(clampedX, clampedY, desiredPos.z);
    }

    /// <summary>
    /// カメラの移動制限範囲を更新する（ステージ切替時に呼ぶ）
    /// </summary>
    public void SetCameraBounds(PolygonCollider2D newBounds)
    {
        _cameraBounds = newBounds;
    }

    void OnDrawGizmosSelected()
    {
        if (_cameraBounds == null) return;
        RecalcCameraSize();

        Bounds b = _cameraBounds.bounds;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(b.center, b.size);

        float minX = b.min.x + _halfWidth;
        float maxX = b.max.x - _halfWidth;
        float minY = b.min.y + _halfHeight;
        float maxY = b.max.y - _halfHeight;

        if (minX < maxX && minY < maxY)
        {
            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
}