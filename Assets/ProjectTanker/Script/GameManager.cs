using R3;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TankStatus      _playerStatus;
    [SerializeField] private GameResultUI    _resultUI;
    [SerializeField] private StageClearUI    _clearUI;
    [SerializeField] private StageStatTracker _statTracker;

    private bool _ended;

    void Start()
    {
        _playerStatus.OnDead
            .Subscribe(_ => EndGame(false))
            .AddTo(this);
    }

    public void TriggerClear() => EndGame(true);

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