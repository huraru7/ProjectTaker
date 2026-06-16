using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/MobilityEffect")]
public class MobilityEffect : SpecialEffect
{
    [SerializeField] private int   speedBonus = 1;
    [SerializeField] private int   turnBonus  = 20;

    public override void Apply(TankStatus status, int stackCount)
    {
        status.getMovementSpeed.Value += speedBonus * stackCount;
        status.getTurnRate.Value      += turnBonus  * stackCount;
    }

    public override void Remove(TankStatus status) { }
}
