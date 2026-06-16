using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/ChainLightningEffect")]
public class ChainLightningEffect : SpecialEffect
{
    [SerializeField] private GameObject chainBulletPrefab;
    [SerializeField] private int   damagePerStack = 4;
    [SerializeField] private int   chainCount     = 2;
    [SerializeField] private float chainRadius    = 5f;

    public override void Apply(TankStatus status, int stackCount)
    {
        var ctrl = status.GetComponent<ChainLightningController>();
        if (ctrl == null) ctrl = status.gameObject.AddComponent<ChainLightningController>();
        ctrl.Setup(chainBulletPrefab, damagePerStack * stackCount,
                   chainCount * stackCount, chainRadius, status);
    }

    public override void Remove(TankStatus status)
    {
        var ctrl = status.GetComponent<ChainLightningController>();
        if (ctrl != null) Destroy(ctrl);
    }
}
