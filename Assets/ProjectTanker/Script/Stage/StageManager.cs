using UnityEngine;

/// <summary>
/// ステージの並び順を一元管理する。Inspectorの_stagesに登録された順に
/// クリアするたびに次のステージへ進む。新しいステージを追加する場合は、
/// GameObjectを作成してMainStageFlowを付け、この配列の末尾に登録するだけでよい。
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private Transform   _playerTransform;
    [SerializeField] private GameObject[] _stages;

    private int _currentIndex;

    public bool HasNextStage => _currentIndex + 1 < _stages.Length;

    public void AdvanceToNextStage()
    {
        if (!HasNextStage) return;

        var currentStage = _stages[_currentIndex];
        _currentIndex++;
        var nextStage = _stages[_currentIndex];

        if (currentStage != null) currentStage.SetActive(false);
        nextStage.SetActive(true);

        var startPoint = nextStage.transform.Find("StartPoint");
        if (_playerTransform != null && startPoint != null)
            _playerTransform.position = startPoint.position;

        nextStage.GetComponent<MainStageFlow>()?.Begin(_playerTransform);

        GameManager.Instance?.ResetForNextStage();
    }
}
