using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/SplitShotEffect")]
public class SplitShotEffect : SpecialEffect
{
    [SerializeField] private GameObject miniPrefab;
    [SerializeField] private int        baseMiniCount = 3;
    [SerializeField] private int        miniDamage    = 2;

    public override void Apply(TankStatus status, int stackCount)
    {
        var ctrl = status.GetComponent<SplitShotController>();
        if (ctrl == null) ctrl = status.gameObject.AddComponent<SplitShotController>();
        int miniCount = baseMiniCount + (stackCount - 1);
        ctrl.Setup(miniPrefab, miniCount, miniDamage, status);
    }

    public override void Remove(TankStatus status)
    {
        var ctrl = status.GetComponent<SplitShotController>();
        if (ctrl != null) Destroy(ctrl);
    }
}
