using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/ArmorEffect")]
public class ArmorEffect : SpecialEffect
{
    [SerializeField] private int hpBonus = 2;

    public override void Apply(TankStatus status, int stackCount)
        => status.getMaxHP.Value += hpBonus * stackCount;

    public override void Remove(TankStatus status) { }
}
