using System;
using UnityEngine;

// ============================================================
// ExpertAI — 総合エキスパートAI
//
// 戦略:
//   ChaseAndFireAI の上位版。理想距離を保ちながら戦い、
//   プレイヤーの移動速度から着弾先を予測して先読み射撃を行う。
//   バレルを予測位置方向へ向けて射撃精度を高める。
//   直接射撃が困難な場合は壁反射を計算して間接攻撃も行う。
//   壁は GetSteeringDir で自動回避する。
//
// 推奨パラメータ（難易度3ベースライン）:
//   - detectionRange: 14 — 探知範囲
//   - preferredDistance: 6 — 維持したい戦闘距離
//   - margin: 1.5 — 距離帯の誤差許容
//   - predictionTime: 0.4 — 弾の予測リードタイム（秒）
//   - fireAngleThreshold: 12 — 直接射撃の角度許容範囲（度）
//   - bounceAngleTolerance: 10 — 反射射撃の角度許容範囲（度）
//   - bounceCheckInterval: 0.5 — 反射方向の再計算間隔（秒）
//   - difficulty: 1〜5（3 がベースライン）
// ============================================================
[Serializable]
public class ExpertAI : EnemyAIBase
{
    [SerializeField] private float detectionRange = 14f;
    [SerializeField] private float preferredDistance = 6f;
    [SerializeField] private float margin = 1.5f;
    [SerializeField] private float predictionTime = 0.4f;
    [SerializeField] private float fireAngleThreshold = 12f;
    [SerializeField] private float bounceAngleTolerance = 10f;
    [SerializeField] private float bounceCheckInterval = 0.5f;
    [SerializeField] private int difficulty = 3;

    private Rigidbody2D _playerRb;
    private Vector2 _bounceCacheDir;
    private float _bounceCacheTimer;
    private float _bounceCheckTimer;

    private float _effectiveFireAngle;
    private float _effectivePredictionTime;
    private float _effectiveDetectionRange;
    private bool _useBounceShots;

    public override int GetDifficulty() => difficulty;

    public override void OnInitialize(EnemyManager manager)
    {
        if (manager.PlayerTransform != null)
            _playerRb = manager.PlayerTransform.GetComponent<Rigidbody2D>();
        _bounceCacheTimer = 0f;
        _bounceCheckTimer = 0f;

        int d = Mathf.Clamp(difficulty, 1, 5) - 1;
        _effectiveFireAngle      = fireAngleThreshold * AIUtil.AngleScale[d];
        _effectivePredictionTime = predictionTime * AIUtil.RangeScale[d];  // 難易度高いほど予測が正確
        _effectiveDetectionRange = detectionRange * AIUtil.RangeScale[d];
        _useBounceShots          = AIUtil.UsesBounceShots(difficulty);
    }

    public override void UpdateAI(EnemyManager manager)
    {
        if (manager.PlayerTransform == null) return;

        _bounceCacheTimer -= Time.deltaTime;
        _bounceCheckTimer -= Time.deltaTime;

        Vector2 selfPos   = manager.SelfTransform.position;
        Vector2 playerPos = manager.PlayerTransform.position;
        Vector2 toPlayer  = playerPos - selfPos;
        float distance    = toPlayer.magnitude;

        if (distance > _effectiveDetectionRange)
        {
            manager.Move(Vector2.zero);
            return;
        }

        Vector2 dir = toPlayer / distance;

        // 3段階距離管理（壁回避つき）
        if (distance > preferredDistance + margin)
            manager.Move(AIUtil.GetSteeringDir(selfPos, dir));
        else if (distance < preferredDistance - margin)
            manager.Move(AIUtil.GetSteeringDir(selfPos, -dir));
        else
            manager.Move(Vector2.zero);

        // 予測位置方向へバレルをエイム
        Vector2 predictedPos = _playerRb != null
            ? playerPos + _playerRb.linearVelocity * _effectivePredictionTime
            : playerPos;
        Vector2 toPredicted = predictedPos - selfPos;

        manager.AimBarrel(toPredicted.normalized);

        // 直接射撃チェック（毎フレーム・軽量）
        if (Vector2.Angle(manager.BarrelAimDirection, toPredicted) <= _effectiveFireAngle)
        {
            manager.Fire();
            return;
        }

        if (!_useBounceShots) return;

        // 壁反射の再計算（インターバルごと）
        if (_bounceCheckTimer <= 0f)
        {
            _bounceCheckTimer = bounceCheckInterval;
            if (AIUtil.TryGetBounceShot(selfPos, playerPos, bounceAngleTolerance, out Vector2 bounceDir))
            {
                _bounceCacheDir   = bounceDir;
                _bounceCacheTimer = bounceCheckInterval + 0.1f;
            }
        }

        // キャッシュした壁反射方向で射撃
        if (_bounceCacheTimer > 0f)
            manager.FireInDirection(_bounceCacheDir);
    }
}
