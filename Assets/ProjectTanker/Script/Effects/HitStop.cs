using System.Collections;
using UnityEngine;

/// <summary>
/// 爆発時に Time.timeScale を一時的に下げる「ヒットストップ」演出。
/// WaitForSecondsRealtime を使うためフレームレート非依存で動作する。
/// </summary>
public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    void Awake() => Instance = this;

    /// <summary>
    /// ヒットストップを実行する。
    /// timeScale を 0.15 に落とし、realtime で duration 秒後に 1 へ戻す。
    /// </summary>
    public void Execute(float duration = 0.08f)
    {
        StopAllCoroutines();
        StartCoroutine(DoHitStop(duration));
    }

    /// <summary>
    /// 実行中のヒットストップを、timeScaleを1に戻さずに止める。
    /// クリア/ゲームオーバーでポーズする直前に呼び、遅延復元による上書きを防ぐ。
    /// </summary>
    public void CancelWithoutRestoring() => StopAllCoroutines();

    private IEnumerator DoHitStop(float duration)
    {
        Time.timeScale = 0.15f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
