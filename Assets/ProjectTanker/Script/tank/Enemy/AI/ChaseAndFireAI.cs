using System;
using UnityEngine;

// ============================================================
// ChaseAndFireAI — 距離管理型追跡AI
//
// 戦略:
//   プレイヤーを探知すると preferredDistance まで接近し、
//   その距離を維持しながら射角が合い次第射撃する。
//   近づきすぎると後退して戦闘距離を保つ。
//
// 推奨パラメータ:
//   - detectionRange: 10 — 追跡を開始する距離
//   - preferredDistance: 5 — 維持したい戦闘距離
//   - margin: 1 — 距離帯の誤差許容（この範囲内なら停止）
//   - fireAngleThreshold: 15 — 射撃を行う正面角度の許容範囲（度）
// ============================================================
[Serializable]
public class ChaseAndFireAI : EnemyAIBase
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float margin = 1f;
    [SerializeField] private float fireAngleThreshold = 15f;

    public override void OnInitialize(EnemyManager manager) { }

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

        if (distance > preferredDistance + margin)
            manager.Move(dir);
        else if (distance < preferredDistance - margin)
            manager.Move(-dir);
        else
            manager.Move(Vector2.zero);

        if (Vector2.Angle(manager.SelfTransform.up, toPlayer) <= fireAngleThreshold)
            manager.Fire();
    }
}
