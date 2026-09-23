using R3;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TankStatus      _playerStatus;
    [SerializeField] private GameResultUI    _resultUI;
    [SerializeField] private StageClearUI    _clearUI;
    [SerializeField] private StageStatTracker _statTracker;

    private bool _ended;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _playerStatus.OnDead
            .Subscribe(_ => EndGame(false))
            .AddTo(this);
    }

    public void TriggerClear() => EndGame(true);

    /// <summary>
    /// 次のステージへ進む際に呼び出し、クリア/ゲームオーバー判定を再度受け付けられる状態に戻す。
    /// </summary>
    public void ResetForNextStage()
    {
        _ended = false;
        Time.timeScale = 1f;
        _statTracker?.ResetAndStart();
    }

    private void EndGame(bool isWin)
    {
        if (_ended) return;
        _ended = true;
        _statTracker?.StopTracking();
        if (isWin && _clearUI != null)
            _clearUI.Show(
                _statTracker?.ShotCount ?? 0,
                _statTracker?.ElapsedSeconds ?? 0f,
                _statTracker?.TraveledMeters ?? 0f);
        else
            _resultUI.Show(isWin);
        Time.timeScale = 0f;
    }
}