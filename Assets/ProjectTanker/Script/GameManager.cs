using R3;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TankStatus   _playerStatus;
    [SerializeField] private GameResultUI _resultUI;

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
        Time.timeScale = 0f;
        _resultUI.Show(isWin);
    }
}