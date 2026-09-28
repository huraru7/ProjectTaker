using System.Collections;
using UnityEngine;

/// <summary>
/// 本編ステージの汎用進行を制御する。TutorialFlowと異なり会話演出等は持たず、
/// ゴール地点への到達のみを検知してGameManagerへクリアを通知する。
/// GoalMarkerは自身の子オブジェクトから自動で探すため、追加のInspector配線は不要。
/// </summary>
public class MainStageFlow : MonoBehaviour
{
    [SerializeField] private float _targetRadius = 1.5f;

    [Tooltip("ゴールへ向かう途中でプレイヤーに提示する経由地点。空でもよい。配列順に誘導する")]
    [SerializeField] private Transform[] _waypoints;

    private static readonly Color IndicatorColor = new(0.961f, 0.773f, 0.094f);

    private Transform _goalMarker;
    private Transform _playerTransform;
    private Transform _trackedMarker;

    void Awake()
    {
        _goalMarker = transform.Find("GoalMarker");
    }

    void OnDisable()
    {
        if (_trackedMarker != null)
            OffScreenIndicator.Instance?.Unregister(_trackedMarker);
    }

    /// <summary>
    /// StageManagerがこのステージを有効化した直後に呼び出す。
    /// </summary>
    public void Begin(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        StartCoroutine(WatchGoal());
    }

    private IEnumerator WatchGoal()
    {
        if (_goalMarker == null) yield break;

        foreach (var waypoint in _waypoints)
        {
            if (waypoint == null) continue;
            yield return WatchMarker(waypoint);
        }

        yield return WatchMarker(_goalMarker);

        GameManager.Instance?.TriggerClear();
    }

    // 指定地点への画面外インジケーターを表示し、プレイヤーが近づくまで待機する
    private IEnumerator WatchMarker(Transform marker)
    {
        _trackedMarker = marker;
        OffScreenIndicator.Instance?.Register(marker, IndicatorColor);
        yield return new WaitUntil(() =>
            Vector3.Distance(_playerTransform.position, marker.position) < _targetRadius);
        OffScreenIndicator.Instance?.Unregister(marker);
        _trackedMarker = null;
    }
}
