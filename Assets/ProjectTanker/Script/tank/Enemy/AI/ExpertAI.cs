using System;
using UnityEngine;

// ============================================================
// ExpertAI — 総合エキスパートAI
//
// 戦略:
//   ChaseAndFireAI の上位版。理想距離を保ちながら戦い、
//   プレイヤーの移動速度から着弾先を予測して先読み射撃を行う。
//   直接射撃が困難な場合は壁反射を計算して間接攻撃も行う。
//
// 推奨パラメータ:
//   - detectionRange: 14 — 探知範囲
//   - preferredDistance: 6 — 維持したい戦闘距離
//   - margin: 1.5 — 距離帯の誤差許容
//   - predictionTime: 0.4 — 弾の予測リードタイム（秒）
//   - fireAngleThreshold: 12 — 直接射撃の角度許容範囲（度）
//   - bounceAngleTolerance: 10 — 反射射撃の角度許容範囲（度）
//   - bounceCheckInterval: 0.5 — 反射方向の再計算間隔（秒）
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

    private Rigidbody2D _playerRb;
    private Vector2 _bounceCacheDir;
    private float _bounceCacheTimer;
    private float _bounceCheckTimer;

    public override void OnInitialize(EnemyManager manager)
    {
        if (manager.PlayerTransform != null)
            _playerRb = manager.PlayerTransform.GetComponent<Rigidbody2D>();
        _bounceCacheTimer = 0f;
        _bounceCheckTimer = 0f;
    }

    public override void UpdateAI(EnemyManager manager)
    {
        if (manager.PlayerTransform == null) return;

        // タイマーを毎フレーム進める（軽量）
        _bounceCacheTimer -= Time.deltaTime;
        _bounceCheckTimer -= Time.deltaTime;

        Vector2 selfPos = manager.SelfTransform.position;
        Vector2 playerPos = manager.PlayerTransform.position;
        Vector2 toPlayer = playerPos - selfPos;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
        {
            manager.Move(Vector2.zero);
            return;
        }

        // 3段階距離管理
        Vector2 dir = toPlayer / distance;
        if (distance > preferredDistance + margin)
            manager.Move(dir);
        else if (distance < preferredDistance - margin)
            manager.Move(-dir);
        else
            manager.Move(Vector2.zero);

        // プレイヤーの移動から着弾先を予測
        Vector2 predictedPos = _playerRb != null
            ? playerPos + _playerRb.linearVelocity * predictionTime
            : playerPos;
        Vector2 toPredicted = predictedPos - selfPos;

        // 直接射撃チェック（毎フレーム・軽量）
        if (Vector2.Angle(manager.SelfTransform.up, toPredicted) <= fireAngleThreshold)
        {
            manager.Fire();
            return;
        }

        // 壁反射の再計算（インターバルごと）
        if (_bounceCheckTimer <= 0f)
        {
            _bounceCheckTimer = bounceCheckInterval;
            if (AIUtil.TryGetBounceShot(selfPos, playerPos, bounceAngleTolerance, out Vector2 bounceDir))
            {
                _bounceCacheDir = bounceDir;
                _bounceCacheTimer = bounceCheckInterval + 0.1f;
            }
        }

        // キャッシュした壁反射方向で射撃
        if (_bounceCacheTimer > 0f)
            manager.FireInDirection(_bounceCacheDir);
    }
}
