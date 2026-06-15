using UnityEngine;

public class TrackDustEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _leftDust;
    [SerializeField] private ParticleSystem _rightDust;
    [SerializeField] private Rigidbody2D    _rb;
    [SerializeField] private Material       _dustMaterial;
    [SerializeField] private float          _minSpeed        = 0.5f;
    [SerializeField] private float          _maxEmissionRate = 30f;

    private ParticleSystem.EmissionModule _leftEmission;
    private ParticleSystem.EmissionModule _rightEmission;

    void Awake()
    {
        SetupSystem(_leftDust);
        SetupSystem(_rightDust);
    }

    void Start()
    {
        _leftEmission  = _leftDust.emission;
        _rightEmission = _rightDust.emission;
    }

    void Update()
    {
        float speed    = _rb.linearVelocity.magnitude;
        bool  isMoving = speed > _minSpeed;

        _leftEmission.enabled  = isMoving;
        _rightEmission.enabled = isMoving;

        if (isMoving)
        {
            float rate = Mathf.Lerp(10f, _maxEmissionRate, speed / 5f);
            _leftEmission.rateOverTime  = rate;
            _rightEmission.rateOverTime = rate;

            // 走行方向と逆向きにパーティクルを流してトレイル表現
            Vector2 trailVelocity = -_rb.linearVelocity * 0.4f;
            SetVelocity(_leftDust,  trailVelocity);
            SetVelocity(_rightDust, trailVelocity);
        }
    }

    private void SetupSystem(ParticleSystem ps)
    {
        var main = ps.main;
        // ワールド座標系で動かす（タンクの回転に追従させない）
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        // 放出方向の初速を0にして velocityOverLifetime だけで方向制御
        main.startSpeed = new ParticleSystem.MinMaxCurve(0f);

        if (_dustMaterial != null)
            ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = _dustMaterial;

        var vol = ps.velocityOverLifetime;
        vol.enabled = true;
        vol.space   = ParticleSystemSimulationSpace.World;
        vol.z       = new ParticleSystem.MinMaxCurve(0f); // Z方向には動かさない
    }

    private static void SetVelocity(ParticleSystem ps, Vector2 velocity)
    {
        var vol = ps.velocityOverLifetime;
        vol.x = new ParticleSystem.MinMaxCurve(velocity.x);
        vol.y = new ParticleSystem.MinMaxCurve(velocity.y);
    }
}
