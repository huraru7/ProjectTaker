using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/AmmoEffect")]
public class AmmoEffect : SpecialEffect
{
    [SerializeField] private int   magBonus        = 1;
    [SerializeField] private float reloadReduction = 0.3f;
    [SerializeField] private float minReloadTime   = 0.2f;

    public override void Apply(TankStatus status, int stackCount)
    {
        status.getMagazineCapacity.Value += magBonus * stackCount;
        status.getReloadTime.Value = Mathf.Max(
            minReloadTime,
            status.getReloadTime.Value - reloadReduction * stackCount);
    }

    public override void Remove(TankStatus status) { }
}
