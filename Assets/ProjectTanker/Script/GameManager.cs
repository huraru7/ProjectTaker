using R3;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TankStatus   _playerStatus;
    [SerializeField] private Transform    _enemyContainer;
    [SerializeField] private GameResultUI _resultUI;

    private bool _ended;

    void Start()
    {
        _playerStatus.OnDead
            .Subscribe(_ => EndGame(false))
            .AddTo(this);
    }

    void Update()
    {
        if (_ended) return;
        if (AllEnemiesDead()) EndGame(true);
    }

    private bool AllEnemiesDead()
    {
        if (_enemyContainer.childCount == 0) return false;
        foreach (Transform child in _enemyContainer)
            if (child.gameObject.activeInHierarchy) return false;
        return true;
    }

    private void EndGame(bool isWin)
    {
        _ended = true;
        Time.timeScale = 0f;
        _resultUI.Show(isWin);
    }
}