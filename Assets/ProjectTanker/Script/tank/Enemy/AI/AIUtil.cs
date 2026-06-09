using UnityEngine;

/// <summary>
/// 壁反射ショットの計算ユーティリティ。
/// 毎フレームではなく射撃タイマー満了時のみ呼ぶこと（レイキャストコスト対策）。
/// </summary>
public static class AIUtil
{
    private const int ScanSteps = 24;   // 15°刻み = 24方向
    private const float MaxRayDist = 25f;

    /// <summary>
    /// from から壁に当てて反射させ target に到達できる射撃方向を探す。
    /// 見つかれば true を返し shootDir に方向を格納する。
    /// </summary>
    public static bool TryGetBounceShot(Vector2 from, Vector2 target, float angleTolerance, out Vector2 shootDir)
    {
        shootDir = Vector2.zero;

        for (int i = 0; i < ScanSteps; i++)
        {
            float angle = i * (360f / ScanSteps) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // 最初の hit が Wall でなければ弾も先に別のものに当たるため skip
            RaycastHit2D wallHit = Physics2D.Raycast(from, dir, MaxRayDist);
            if (!wallHit || !wallHit.collider.TryGetComponent<Wall>(out _)) continue;

            // 壁からの反射方向を計算
            Vector2 reflected = Vector2.Reflect(dir, wallHit.normal);
            Vector2 bounceOrigin = wallHit.point + wallHit.normal * 0.05f;
            Vector2 toTarget = (target - bounceOrigin).normalized;

            if (Vector2.Angle(reflected, toTarget) > angleTolerance) continue;

            // 反射点 → ターゲット間が別の Wall に遮られていないか確認
            float distToTarget = Vector2.Distance(bounceOrigin, target);
            RaycastHit2D pathCheck = Physics2D.Raycast(bounceOrigin, toTarget, distToTarget);
            if (pathCheck && pathCheck.collider.TryGetComponent<Wall>(out _)) continue;

            shootDir = dir;
            return true;
        }
        return false;
    }
}