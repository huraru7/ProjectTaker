using System;

[Serializable]
public abstract class EnemyAIBase
{
    public abstract void OnInitialize(EnemyManager manager);
    public abstract void UpdateAI(EnemyManager manager);
}
