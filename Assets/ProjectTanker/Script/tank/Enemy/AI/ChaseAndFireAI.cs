using System;
using UnityEngine;

[Serializable]
public class ChaseAndFireAI : EnemyAIBase
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 5f;
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

        manager.Move(toPlayer.normalized);

        if (distance <= attackRange)
        {
            float angle = Vector2.Angle(manager.SelfTransform.up, toPlayer);
            if (angle <= fireAngleThreshold)
                manager.Fire();
        }
    }
}
