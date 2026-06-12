using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private GameObject      _panel;
    [SerializeField] private TextMeshProUGUI _titleText;

    public void Show(bool isWin)
    {
        _panel.SetActive(true);
        _titleText.text = isWin ? "STAGE CLEAR" : "GAME OVER";
    }

    public void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("2_title");
    }
}