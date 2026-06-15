using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private List<TutorialStep> steps = new();

    private static readonly HashSet<string> _completedIds = new();
    private Coroutine _running;

    void Awake() => Instance = this;

    /// <summary>
    /// チュートリアルを開始する。force=true で完了済みでも再実行。
    /// </summary>
    public void StartTutorial(string tutorialId, bool force = false)
    {
        if (!force && _completedIds.Contains(tutorialId)) return;
        if (_running != null) return;
        _running = StartCoroutine(RunTutorialCo(tutorialId));
    }

    private IEnumerator RunTutorialCo(string id)
    {
        foreach (var step in steps)
        {
            if (step.DelayBefore > 0f)
                yield return new WaitForSecondsRealtime(step.DelayBefore);

            NotificationManager.Show(new NotificationData
            {
                Message     = step.Message,
                Icon        = step.Icon,
                AccentColor = new Color(0.961f, 0.773f, 0.094f), // Tutorial yellow
                Duration    = step.Duration,
            });

            if (step.Condition != null)
                yield return new WaitUntil(step.Condition);
            else
                yield return new WaitForSecondsRealtime(step.Duration);
        }

        _completedIds.Add(id);
        _running = null;
        NotificationManager.Success("チュートリアル完了！");
    }
}

[Serializable]
public class TutorialStep
{
    [TextArea(2, 4)] public string Message;
    public Sprite Icon;
    public float  Duration    = 5f;
    public float  DelayBefore = 0f;

    [NonSerialized] public Func<bool> Condition; // コードから設定する条件付き待機
}
