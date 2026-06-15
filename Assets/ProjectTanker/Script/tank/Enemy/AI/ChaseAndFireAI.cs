using System;
using UnityEngine;

// ============================================================
// ChaseAndFireAI — 距離管理型追跡AI
//
// 戦略:
//   プレイヤーを探知すると preferredDistance まで接近し、
//   その距離を維持しながらバレルを向けて射撃する。
//   近づきすぎると後退して戦闘距離を保つ。
//   壁は GetSteeringDir で自動回避する。
//
// 推奨パラメータ（難易度3ベースライン）:
//   - detectionRange: 10 — 追跡を開始する距離
//   - preferredDistance: 5 — 維持したい戦闘距離
//   - margin: 1 — 距離帯の誤差許容
//   - fireAngleThreshold: 15 — 射撃を行う角度許容範囲（度）
//   - difficulty: 1〜5（3 がベースライン）
// ============================================================
[Serializable]
public class ChaseAndFireAI : EnemyAIBase
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float margin = 1f;
    [SerializeField] private float fireAngleThreshold = 15f;
    [SerializeField] private int difficulty = 3;

    private float _effectiveFireAngle;
    private float _effectiveDetectionRange;
    private bool _useBounceShots;

    public override int GetDifficulty() => difficulty;

    public override void OnInitialize(EnemyManager manager)
    {
        int d = Mathf.Clamp(difficulty, 1, 5) - 1;
        _effectiveFireAngle      = fireAngleThreshold * AIUtil.AngleScale[d];
        _effectiveDetectionRange = detectionRange * AIUtil.RangeScale[d];
        _useBounceShots          = AIUtil.UsesBounceShots(difficulty);
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

        // 距離管理（壁回避を適用した移動方向）
        if (distance > preferredDistance + margin)
            manager.Move(AIUtil.GetSteeringDir(selfPos, dir));
        else if (distance < preferredDistance - margin)
            manager.Move(AIUtil.GetSteeringDir(selfPos, -dir));
        else
            manager.Move(Vector2.zero);

        // バレルをプレイヤー方向へ向ける
        manager.AimBarrel(dir);

        // 直接射撃
        if (Vector2.Angle(manager.BarrelAimDirection, toPlayer) <= _effectiveFireAngle)
        {
            manager.Fire();
            return;
        }

        // バウンスショット（難易度 3 以上かつ直接射撃 NG 時）
        if (_useBounceShots && AIUtil.TryGetBounceShot(selfPos, manager.PlayerTransform.position, _effectiveFireAngle, out Vector2 bounceDir))
            manager.FireInDirection(bounceDir);
    }
}
