using UnityEngine;

/// <summary>
/// ステージの並び順を一元管理する。Inspectorの_stagesに登録された順に
/// クリアするたびに次のステージへ進む。新しいステージを追加する場合は、
/// GameObjectを作成してMainStageFlowを付け、この配列の末尾に登録するだけでよい。
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private Transform    _playerTransform;
    [SerializeField] private GameObject[] _stages;

    [Tooltip("チュートリアル進行を持つオブジェクト。_stages[0](tutorials)以外がアクティブな状態で開始された場合に無効化する")]
    [SerializeField] private GameObject _tutorialRoot;

    private int _currentIndex;
    private bool _needsInitialEntry;

    public bool HasNextStage => _currentIndex + 1 < _stages.Length;

    void Awake()
    {
        _currentIndex = FindActiveStageIndex();

        // Editor上でtutorials以外のステージがアクティブなまま再生された場合、
        // 動作確認用にチュートリアルをスキップしてそのステージから直接開始する。
        // チュートリアルの無効化はAwake()フェーズ中(Start()が呼ばれる前)に
        // 行う必要があるためここで行うが、GameManager.Instanceの初期化順は
        // 保証されないため、それに依存する処理はStart()側で行う。
        if (_currentIndex > 0)
        {
            if (_tutorialRoot != null) _tutorialRoot.SetActive(false);
            _needsInitialEntry = true;
        }
    }

    void Start()
    {
        if (!_needsInitialEntry) return;

        EnterStage(_stages[_currentIndex]);
        GameManager.Instance?.ResetForNextStage();
    }

    private int FindActiveStageIndex()
    {
        for (int i = 0; i < _stages.Length; i++)
            if (_stages[i] != null && _stages[i].activeSelf) return i;
        return 0;
    }

    public void AdvanceToNextStage()
    {
        if (!HasNextStage) return;

        var currentStage = _stages[_currentIndex];
        _currentIndex++;
        var nextStage = _stages[_currentIndex];

        if (nextStage == null)
        {
            Debug.LogWarning($"[StageManager] _stages[{_currentIndex}] が未設定です。インスペクタで紐付けてください。");
            _currentIndex--;
            return;
        }

        if (currentStage != null) currentStage.SetActive(false);
        EnterStage(nextStage);

        GameManager.Instance?.ResetForNextStage();
    }

    private void EnterStage(GameObject stage)
    {
        stage.SetActive(true);

        var startPoint = stage.transform.Find("StartPoint");
        if (_playerTransform != null && startPoint != null)
            _playerTransform.position = startPoint.position;

        stage.GetComponent<MainStageFlow>()?.Begin(_playerTransform);
    }
}
