using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Data/AI/NullAI")]
public class NullAI : EnemyAIBase
{
    public override void OnInitialize(EnemyManager manager) { }
    public override void UpdateAI(EnemyManager manager) { }
    public override int GetDifficulty() => 0;
}
