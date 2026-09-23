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

    private Transform _goalMarker;
    private Transform _playerTransform;

    void Awake()
    {
        _goalMarker = transform.Find("GoalMarker");
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

        OffScreenIndicator.Instance?.Register(_goalMarker, new Color(0.961f, 0.773f, 0.094f));
        yield return new WaitUntil(() =>
            Vector3.Distance(_playerTransform.position, _goalMarker.position) < _targetRadius);
        OffScreenIndicator.Instance?.Unregister(_goalMarker);

        GameManager.Instance?.TriggerClear();
    }
}
