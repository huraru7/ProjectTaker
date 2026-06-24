using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/SplitShotEffect")]
public class SplitShotEffect : SpecialEffect
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float      baseRadius = 2f;
    [SerializeField] private int        baseDamage = 5;

    public override void Apply(TankStatus status, int stackCount)
    {
        var ctrl = status.GetComponent<SplitShotController>();
        if (ctrl == null) ctrl = status.gameObject.AddComponent<SplitShotController>();
        float radius = baseRadius + (stackCount - 1) * 0.5f;
        ctrl.Setup(explosionPrefab, radius, baseDamage * stackCount, status);
    }

    public override void Remove(TankStatus status)
    {
        var ctrl = status.GetComponent<SplitShotController>();
        if (ctrl != null) Destroy(ctrl);
    }
}
