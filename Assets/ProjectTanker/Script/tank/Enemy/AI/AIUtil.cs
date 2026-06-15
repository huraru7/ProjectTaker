using UnityEngine;

/// <summary>
/// AI 共通ユーティリティ。壁反射ショット計算・壁回避ステアリング・難易度テーブル。
/// </summary>
public static class AIUtil
{
    // ----------------------------------------------------------------
    // 難易度スケールテーブル（難易度3がベースライン = 1.0）
    // ----------------------------------------------------------------

    // 射撃角度倍率: 大きいほど易しい（広い角度で撃てる）
    public static readonly float[] AngleScale = { 3.0f, 2.0f, 1.0f, 0.6f, 0.3f };

    // 探知距離倍率: 小さいほど易しい（見つけにくい）
    public static readonly float[] RangeScale = { 0.6f, 0.8f, 1.0f, 1.2f, 1.5f };

    // エイム時間・射撃間隔倍率: 大きいほど易しい（反応が遅い）
    public static readonly float[] TimerScale = { 3.0f, 2.0f, 1.0f, 0.6f, 0.3f };

    // 難易度 3 以上でバウンスショットを使用する
    public static bool UsesBounceShots(int difficulty) => difficulty >= 3;

    // ----------------------------------------------------------------
    // 壁回避ステアリング
    // ----------------------------------------------------------------

    /// <summary>
    /// desiredDir 方向に壁があれば回避した移動方向を返す。1〜3 レイキャストのみで軽量。
    /// </summary>
    public static Vector2 GetSteeringDir(Vector2 from, Vector2 desiredDir, float checkDist = 2f)
    {
        if (desiredDir == Vector2.zero) return Vector2.zero;
        Vector2 fwd = desiredDir.normalized;

        // 正面が通れる → そのまま進む（1 レイで早期リターン）
        if (!HitsWall(from, fwd, checkDist))
            return fwd;

        // 正面が壁 → 左右 45° をチェック
        Vector2 left  = RotateVec(fwd,  45f);
        Vector2 right = RotateVec(fwd, -45f);
        bool leftBlocked  = HitsWall(from, left,  checkDist * 0.8f);
        bool rightBlocked = HitsWall(from, right, checkDist * 0.8f);

        if (!leftBlocked  && rightBlocked)  return left;
        if (!rightBlocked && leftBlocked)   return right;
        if (!leftBlocked)                   return left;  // 両側クリア → 左を優先

        // 全方向塞がれ → 90° 回転して突破
        return RotateVec(fwd, 90f);
    }

    private static bool HitsWall(Vector2 from, Vector2 dir, float dist)
    {
        RaycastHit2D hit = Physics2D.Raycast(from, dir, dist);
        return hit && hit.collider.TryGetComponent<Wall>(out _);
    }

    private static Vector2 RotateVec(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    // ----------------------------------------------------------------
    // 壁反射ショット計算
    // ----------------------------------------------------------------

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