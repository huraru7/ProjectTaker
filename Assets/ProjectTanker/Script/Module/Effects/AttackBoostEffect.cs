using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/AttackBoostEffect")]
public class AttackBoostEffect : SpecialEffect
{
    [SerializeField] private int attackBonus = 5;

    public override void Apply(TankStatus status, int stackCount)
        => status.getBaseAttackPower.Value += attackBonus * stackCount;

    public override void Remove(TankStatus status) { }
}
