using UnityEngine;

public class SceneBGMPlayer : MonoBehaviour
{
    public enum BGMTrack { Title, Game }

    [SerializeField] private BGMTrack track;

    void Start()
    {
        if (track == BGMTrack.Title) AudioManager.Instance?.PlayTitleBGM();
        else                         AudioManager.Instance?.PlayGameBGM();
    }
}
