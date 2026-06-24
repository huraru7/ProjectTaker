using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create TankData")]
public class TankData : ScriptableObject
{
    [Header("Tanker Status")]
    [Tooltip("最大HP")] public int maxHP;
    [Tooltip("基礎攻撃力")] public int baseAttackPower;
    [Tooltip("移動速度")] public int movementSpeed;
    [Tooltip("旋回速度")] public int turnRate;

    [Header("Bullet Status")]
    [Tooltip("弾速")] public float bulletSpeed;
    [Tooltip("総弾数")] public int magazineCapacity;
    [Tooltip("リロード時間")] public float reloadTime;

    [Header("Barrel")]
    [Tooltip("バレル回転速度（度/秒）")] public float barrelTurnRate = 120f;

    [Header("Camera")]
    [Tooltip("カメラの表示範囲（Orthographic Size）")] public float cameraViewSize = 5f;
}