using R3;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TankStatus))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private TankStatus _tankStatus;

    private Rigidbody2D _rb;
    private float _moveSpeed;
    private float _turnRate;
    private Vector2 _moveInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_tankStatus == null) _tankStatus = GetComponent<TankStatus>();
    }

    void Start()
    {
        _moveSpeed = _tankStatus.getMovementSpeed.Value;
        _turnRate = _tankStatus.getTurnRate.Value;

        _tankStatus.getMovementSpeed.Subscribe(v => _moveSpeed = v).AddTo(this);
        _tankStatus.getTurnRate.Subscribe(v => _turnRate = v).AddTo(this);
    }

    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }

    void FixedUpdate()
    {
        if (_moveInput != Vector2.zero)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, -90f + Mathf.Atan2(_moveInput.y, _moveInput.x) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * _turnRate);
        }

        _rb.linearVelocity = transform.up * _moveSpeed * (_moveInput == Vector2.zero ? 0f : 1f);
    }
}
