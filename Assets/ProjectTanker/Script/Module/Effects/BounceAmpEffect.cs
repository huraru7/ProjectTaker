using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/BounceAmpEffect")]
public class BounceAmpEffect : SpecialEffect
{
    [SerializeField] private int bonusPerBounce = 3;

    public override void Apply(TankStatus status, int stackCount)
    {
        var ctrl = status.GetComponent<BounceAmpController>();
        if (ctrl == null) ctrl = status.gameObject.AddComponent<BounceAmpController>();
        ctrl.Setup(bonusPerBounce * stackCount);
    }

    public override void Remove(TankStatus status)
    {
        var ctrl = status.GetComponent<BounceAmpController>();
        if (ctrl != null) Destroy(ctrl);
    }
}
