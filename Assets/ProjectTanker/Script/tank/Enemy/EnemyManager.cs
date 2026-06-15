using R3;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TankStatus _tankStatus;
    [SerializeField] private EnemyMovement _movement;
    [SerializeField] private EnemyBulletManager _bulletManager;

    [Header("Barrel")]
    [SerializeField] private BarrelController _barrel;

    [Header("Target")]
    [SerializeField] private Transform _playerTransform;

    [Header("AI Strategy")]
    [SerializeReference] private EnemyAIBase _ai;

    public TankStatus Status => _tankStatus;
    public Transform PlayerTransform => _playerTransform;
    public Transform SelfTransform => transform;

    void Start()
    {
        _ai?.OnInitialize(this);

        _tankStatus.OnDead
            .Subscribe(_ =>
            {
                ExperienceManager.Instance?.DropXp(transform.position, _ai);
                _ai = null;
                ExplosionEffect.SpawnAt(transform.position);
                gameObject.SetActive(false);
            })
            .AddTo(this);
    }

    void Update()
    {
        _ai?.UpdateAI(this);
    }

    public void Move(Vector2 input)
    {
        _movement.SetMoveInput(input);
    }

    public void Fire()
    {
        _bulletManager.Fire();
    }

    public void FireInDirection(Vector2 direction)
    {
        _bulletManager.FireInDirection(direction);
    }

    public void AimBarrel(Vector2 worldDir) => _barrel.RotateToward(worldDir, _tankStatus.getBarrelTurnRate.Value);

    public Vector2 BarrelAimDirection => _barrel.AimDirection;
}