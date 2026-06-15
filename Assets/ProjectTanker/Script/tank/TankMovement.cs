using R3;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TankStatus))]
public class TankMovement : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnRate = 2f;

    [Header("Barrel")]
    [SerializeField] private BarrelController _barrel;

    [SerializeField] private TankStatus _tankStatus;

    private Rigidbody2D rb;
    private Vector2 move;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (_tankStatus == null) _tankStatus = GetComponent<TankStatus>();
    }

    private void Start()
    {
        moveSpeed = _tankStatus.getMovementSpeed.Value;
        turnRate  = _tankStatus.getTurnRate.Value;

        _tankStatus.getMovementSpeed.Subscribe(v => moveSpeed = v).AddTo(this);
        _tankStatus.getTurnRate.Subscribe(v => turnRate = v).AddTo(this);
    }

    private void Update()
    {
        var kb = Keyboard.current;
        move = new Vector2(
            (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0),
            (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0)
        );

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld  = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        Vector2 dir = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
        if (dir != Vector2.zero)
            _barrel.RotateTo(dir);
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = 0f;

        if (move != Vector2.zero)
        {
            float angle = Vector2.Angle(transform.up, move);
            if (angle >= 135f)
            {
                rb.linearVelocity = -transform.up * moveSpeed;
            }
            else
            {
                Quaternion rot = Quaternion.Euler(0f, 0f, -90f + Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnRate);
                rb.linearVelocity = transform.up * moveSpeed;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
