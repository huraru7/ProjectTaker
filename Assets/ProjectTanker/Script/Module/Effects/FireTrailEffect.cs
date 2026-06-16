using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/FireTrailEffect")]
public class FireTrailEffect : SpecialEffect
{
    [SerializeField] private GameObject flameZonePrefab;
    [SerializeField] private float spawnInterval       = 0.12f;
    [SerializeField] private float flameDuration       = 1.8f;
    [SerializeField] private int   flameDamagePerStack = 3;

    public override void Apply(TankStatus status, int stackCount)
    {
        var ctrl = status.GetComponent<FireTrailController>();
        if (ctrl == null) ctrl = status.gameObject.AddComponent<FireTrailController>();
        ctrl.Setup(flameZonePrefab, spawnInterval, flameDuration,
                   flameDamagePerStack * stackCount, status);
    }

    public override void Remove(TankStatus status)
    {
        var ctrl = status.GetComponent<FireTrailController>();
        if (ctrl != null) Destroy(ctrl);
    }
}
