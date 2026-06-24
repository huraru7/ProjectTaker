using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/AmmoEffect")]
public class AmmoEffect : SpecialEffect
{
    [Tooltip("1スタックあたりのカメラ視野拡大量（Orthographic Size 加算値）")]
    [SerializeField] private float viewSizeBonus = 1.5f;

    public override void Apply(TankStatus status, int stackCount)
        => status.getCameraViewSize.Value += viewSizeBonus * stackCount;

    public override void Remove(TankStatus status) { }
}
