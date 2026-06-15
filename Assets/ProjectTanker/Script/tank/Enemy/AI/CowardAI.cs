using System;
using UnityEngine;

// ============================================================
// CowardAI — 安全確保優先AI
//
// 戦略:
//   プレイヤーが近づくと逃げることに専念し、一切攻撃しない。
//   safeDistance 以上の距離を確保できた時だけ攻撃を試みる。
//   安全距離からバレルを向けて射撃し、壁反射も使用する。
//   壁は GetSteeringDir で自動回避する。
//
// 推奨パラメータ（難易度3ベースライン）:
//   - detectionRange: 16 — 探知範囲
//   - safeDistance: 10 — これ以上離れていれば安全と判断
//   - panicDistance: 7 — これより近いと全力で逃げる
//   - fireInterval: 1.0 — 安全時の射撃間隔（秒）
//   - fireAngleThreshold: 20 — 直接射撃の角度許容範囲（度）
//   - bounceAngleTolerance: 15 — 壁反射時の角度許容範囲（度）
//   - difficulty: 1〜5（3 がベースライン）
// ============================================================
[Serializable]
public class CowardAI : EnemyAIBase
{
    [SerializeField] private float detectionRange = 16f;
    [SerializeField] private float safeDistance = 10f;
    [SerializeField] private float panicDistance = 7f;
    [SerializeField] private float fireInterval = 1.0f;
    [SerializeField] private float fireAngleThreshold = 20f;
    [SerializeField] private float bounceAngleTolerance = 15f;
    [SerializeField] private int difficulty = 3;

    private float _fireTimer;
    private float _effectiveFireAngle;
    private float _effectiveFireInterval;
    private float _effectiveDetectionRange;
    private bool _useBounceShots;

    public override int GetDifficulty() => difficulty;

    public override void OnInitialize(EnemyManager manager)
    {
        int d = Mathf.Clamp(difficulty, 1, 5) - 1;
        _effectiveFireAngle      = fireAngleThreshold * AIUtil.AngleScale[d];
        _effectiveFireInterval   = fireInterval * AIUtil.TimerScale[d];
        _effectiveDetectionRange = detectionRange * AIUtil.RangeScale[d];
        _useBounceShots          = AIUtil.UsesBounceShots(difficulty);
        _fireTimer               = _effectiveFireInterval;
    }

    public override void UpdateAI(EnemyManager manager)
    {
        if (manager.PlayerTransform == null) return;

        Vector2 selfPos  = manager.SelfTransform.position;
        Vector2 toPlayer = (Vector2)manager.PlayerTransform.position - selfPos;
        float distance   = toPlayer.magnitude;

        if (distance > _effectiveDetectionRange)
        {
            manager.Move(Vector2.zero);
            return;
        }

        Vector2 dir = toPlayer / distance;
        bool isSafe = distance >= safeDistance;

        // 移動: panic → 全力逃げ / 不安全 → 安全距離まで後退 / 安全 → 停止
        if (distance < panicDistance)
            manager.Move(AIUtil.GetSteeringDir(selfPos, -dir));
        else if (!isSafe)
            manager.Move(AIUtil.GetSteeringDir(selfPos, -dir));
        else
            manager.Move(Vector2.zero);

        // 射撃は安全な時だけタイマーを進める
        if (!isSafe) return;

        // 安全時はバレルをプレイヤー方向へ向ける
        manager.AimBarrel(dir);

        _fireTimer -= Time.deltaTime;
        if (_fireTimer > 0f) return;

        _fireTimer = _effectiveFireInterval;

        if (Vector2.Angle(manager.BarrelAimDirection, toPlayer) <= _effectiveFireAngle)
        {
            manager.Fire();
            return;
        }

        // 直接射撃 NG → 壁反射（難易度 3 以上）
        if (_useBounceShots && AIUtil.TryGetBounceShot(selfPos, manager.PlayerTransform.position, bounceAngleTolerance, out Vector2 bounceDir))
            manager.FireInDirection(bounceDir);
    }
}
