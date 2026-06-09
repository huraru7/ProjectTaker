using System;
using UnityEngine;

// ============================================================
// SniperAI — 遠距離精密射撃AI
//
// 戦略:
//   snipeRange まで近づいたら停止してエイムを開始する。
//   aimDuration 秒後に直接射撃を試み、角度が合わない場合は
//   壁反射を計算して間接射撃を行う。
//   近づかれると retreatRange を超えるまで後退する。
//
// 推奨パラメータ:
//   - detectionRange: 20 — 広い探知範囲
//   - snipeRange: 13 — 停止して狙い始める距離
//   - retreatRange: 6 — 後退を開始する距離（近寄られたら逃げる）
//   - aimDuration: 1.2 — 射撃前のエイム時間（秒）
//   - fireAngleThreshold: 5 — 精密射撃の角度許容範囲（度）
//   - bounceAngleTolerance: 8 — 壁反射時の角度許容範囲（度）
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

    private float _aimTimer;

    public override void OnInitialize(EnemyManager manager)
    {
        _aimTimer = 0f;
    }

    public override void UpdateAI(EnemyManager manager)
    {
        if (manager.PlayerTransform == null) return;

        Vector2 toPlayer = manager.PlayerTransform.position - manager.SelfTransform.position;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
        {
            manager.Move(Vector2.zero);
            _aimTimer = 0f;
            return;
        }

        Vector2 dir = toPlayer / distance;

        if (distance < retreatRange)
        {
            // プレイヤーに近づかれた → 後退
            manager.Move(-dir);
            _aimTimer = 0f;
            return;
        }

        if (distance > snipeRange)
        {
            // 狙撃ポジションへ接近
            manager.Move(dir);
            _aimTimer = 0f;
            return;
        }

        // 狙撃ポジション: 停止してエイム
        manager.Move(Vector2.zero);
        _aimTimer += Time.deltaTime;

        if (_aimTimer < aimDuration) return;

        // エイム完了 → 射撃判定（タイマー満了時のみ実行）
        _aimTimer = 0f;

        if (Vector2.Angle(manager.SelfTransform.up, toPlayer) <= fireAngleThreshold)
        {
            manager.Fire();
            return;
        }

        // 直接射撃 NG → 壁反射を試みる
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
