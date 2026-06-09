using System;
using UnityEngine;

// ============================================================
// CowardAI — 安全確保優先AI
//
// 戦略:
//   プレイヤーが近づくと逃げることに専念し、一切攻撃しない。
//   safeDistance 以上の距離を確保できた時だけ攻撃を試みる。
//   「安全だと確信できるまでは撃たない」臆病な性格。
//   安全距離からは壁反射も使って狙いを定める。
//
// 推奨パラメータ:
//   - detectionRange: 16 — 探知範囲
//   - safeDistance: 10 — これ以上離れていれば安全と判断
//   - panicDistance: 7 — これより近いと全力で逃げる
//   - fireInterval: 1.0 — 安全時の射撃間隔（秒）
//   - fireAngleThreshold: 20 — 直接射撃の角度許容範囲（度）
//   - bounceAngleTolerance: 15 — 壁反射時の角度許容範囲（度）
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

    private float _fireTimer;

    public override void OnInitialize(EnemyManager manager)
    {
        _fireTimer = fireInterval;
    }

    public override void UpdateAI(EnemyManager manager)
    {
        if (manager.PlayerTransform == null) return;

        Vector2 toPlayer = manager.PlayerTransform.position - manager.SelfTransform.position;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
        {
            manager.Move(Vector2.zero);
            return;
        }

        Vector2 dir = toPlayer / distance;
        bool isSafe = distance >= safeDistance;

        // 移動: panic → 全力逃げ / 不安全 → 安全距離まで後退 / 安全 → 停止
        if (distance < panicDistance)
            manager.Move(-dir);
        else if (!isSafe)
            manager.Move(-dir);
        else
            manager.Move(Vector2.zero);

        // 射撃は安全な時だけタイマーを進める
        if (!isSafe) return;

        _fireTimer -= Time.deltaTime;
        if (_fireTimer > 0f) return;

        _fireTimer = fireInterval;

        if (Vector2.Angle(manager.SelfTransform.up, toPlayer) <= fireAngleThreshold)
        {
            manager.Fire();
            return;
        }

        // 直接射撃 NG → 壁反射を試みる（タイマー満了時のみ実行）
        if (AIUtil.TryGetBounceShot(
            manager.SelfTransform.position,
            manager.PlayerTransform.position,
            bounceAngleTolerance,
            out Vector2 bounceDir))
        {
            manager.FireInDirection(bounceDir);
        }
    }
}
