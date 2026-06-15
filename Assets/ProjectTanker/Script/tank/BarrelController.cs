using UnityEngine;

public class BarrelController : MonoBehaviour
{
    [SerializeField] private float _muzzleOffset = 1f;

    // ワールド方向へ向かってバレルを回転（AI・自動追従用）
    public void RotateToward(Vector2 worldDir, float rate)
    {
        if (worldDir == Vector2.zero) return;
        float target = Mathf.Atan2(worldDir.y, worldDir.x) * Mathf.Rad2Deg - 90f;
        float current = transform.eulerAngles.z;
        float next = Mathf.MoveTowardsAngle(current, target, rate * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, next);
    }

    // 角度を直接加算（プレイヤー入力用）
    public void RotateDelta(float deltaDeg)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, transform.eulerAngles.z + deltaDeg);
    }

    // ワールド方向へ即時回転（遅延なし・マウス追従用）
    public void RotateTo(Vector2 worldDir)
    {
        if (worldDir == Vector2.zero) return;
        float target = Mathf.Atan2(worldDir.y, worldDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, target);
    }

    // 現在のバレルが向いている方向（ワールド空間）
    public Vector2 AimDirection => transform.up;

    // コライダー重複による自傷を防ぐため、バレル先端方向へオフセットした発射位置
    public Vector2 MuzzlePosition => (Vector2)transform.position + AimDirection * _muzzleOffset;
}
