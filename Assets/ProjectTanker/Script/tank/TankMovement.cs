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

    // [Header("Tank")]
    // [SerializeField] private GameObject tankBarrel;

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
        turnRate = _tankStatus.getTurnRate.Value;

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
    }
    private void FixedUpdate()
    {
        Debug.Log($"move: {move}, moveSpeed: {moveSpeed}, turnRate: {turnRate}");
        if (move != Vector2.zero)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, -90f + Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnRate);
        }

        rb.linearVelocity = transform.up * moveSpeed * (move == Vector2.zero ? 0f : 1f);
    }
}
