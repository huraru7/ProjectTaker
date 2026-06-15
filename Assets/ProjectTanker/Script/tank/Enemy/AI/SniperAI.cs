using System;
using UnityEngine;

// ============================================================
// SniperAI — 遠距離精密射撃AI
//
// 戦略:
//   snipeRange まで近づいたら停止してバレルでエイムを開始する。
//   aimDuration 秒後に直接射撃を試み、角度が合わない場合は
//   壁反射を計算して間接射撃を行う。
//   近づかれると retreatRange を超えるまで後退する。
//   壁は GetSteeringDir で自動回避する。
//
// 推奨パラメータ（難易度3ベースライン）:
//   - detectionRange: 20 — 広い探知範囲
//   - snipeRange: 13 — 停止して狙い始める距離
//   - retreatRange: 6 — 後退を開始する距離
//   - aimDuration: 1.2 — 射撃前のエイム時間（秒）
//   - fireAngleThreshold: 5 — 精密射撃の角度許容範囲（度）
//   - bounceAngleTolerance: 8 — 壁反射時の角度許容範囲（度）
//   - difficulty: 1〜5（3 がベースライン）
// ============================================================
[Serializable]
public class SniperAI : EnemyAIBase
{
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float snipeRange = 13f;
    [SerializeField] private float retreatRange = 6f;
    [SerializeField] private float fireAngleThreshold = 5f;
    [SerializeField] private float bounceAngleTolerance = 8f;
    [SerializeField] private float aimDuration = 1.2f;
    [SerializeField] private int difficulty = 3;

    private float _aimTimer;
    private float _effectiveFireAngle;
    private float _effectiveAimDuration;
    private float _effectiveDetectionRange;
    private bool _useBounceShots;

    public override int GetDifficulty() => difficulty;

    public override void OnInitialize(EnemyManager manager)
    {
        _aimTimer = 0f;
        int d = Mathf.Clamp(difficulty, 1, 5) - 1;
        _effectiveFireAngle      = fireAngleThreshold * AIUtil.AngleScale[d];
        _effectiveAimDuration    = aimDuration * AIUtil.TimerScale[d];
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
            _aimTimer = 0f;
            return;
        }

        Vector2 dir = toPlayer / distance;

        if (distance < retreatRange)
        {
            // 近づかれた → 後退（壁回避つき）
            manager.Move(AIUtil.GetSteeringDir(selfPos, -dir));
            _aimTimer = 0f;
            return;
        }

        if (distance > snipeRange)
        {
            // 狙撃ポジションへ接近（壁回避つき）
            manager.Move(AIUtil.GetSteeringDir(selfPos, dir));
            _aimTimer = 0f;
            return;
        }

        // 狙撃ポジション: 停止してバレルをエイム
        manager.Move(Vector2.zero);
        manager.AimBarrel(dir);
        _aimTimer += Time.deltaTime;

        if (_aimTimer < _effectiveAimDuration) return;

        // エイム完了 → 射撃判定
        _aimTimer = 0f;

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
