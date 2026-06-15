using System.Collections.Generic;
using R3;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] private NotificationEntry entryPrefab;
    [SerializeField] private float             defaultDuration = 3.5f;
    [SerializeField] private int               maxEntries      = 5;
    [SerializeField] private float             entrySpacing    = 8f;
    [SerializeField] private float             topMargin       = 16f;
    [SerializeField] private RectTransform     container;

    private readonly List<NotificationEntry>   _active  = new();
    private readonly Subject<NotificationData> _subject = new();

    void Awake()
    {
        Instance = this;
        _subject.Subscribe(ShowInternal).AddTo(this);
    }

    // ----------------------------------------------------------------
    // Static API
    // ----------------------------------------------------------------

    public static void Show(string message) =>
        Show(NotificationData.Info(message));

    public static void Show(NotificationData data) =>
        Instance?._subject.OnNext(data);

    public static void Tutorial(string message, Sprite icon = null) =>
        Show(NotificationData.Tutorial(message, icon));

    public static void Warning(string message, Sprite icon = null) =>
        Show(NotificationData.Warning(message, icon));

    public static void Success(string message, Sprite icon = null) =>
        Show(NotificationData.Success(message, icon));

    // ----------------------------------------------------------------
    // Internal
    // ----------------------------------------------------------------

    private void ShowInternal(NotificationData data)
    {
        // 上限超過時は最古（末尾）を削除
        if (_active.Count >= maxEntries)
        {
            int last = _active.Count - 1;
            Destroy(_active[last].gameObject);
            _active.RemoveAt(last);
        }

        var entry = Instantiate(entryPrefab, container);
        var rt    = entry.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(0f, 1f);
        rt.pivot            = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(0f, -topMargin);

        entry.Initialize(data, defaultDuration);
        _active.Insert(0, entry);

        RearrangeEntries();
    }

    // NotificationEntry の OnDestroy から呼ばれる
    public void OnEntryDestroyed(NotificationEntry entry)
    {
        _active.Remove(entry);
        RearrangeEntries();
    }

    private void RearrangeEntries()
    {
        float y = -topMargin;
        foreach (var e in _active)
        {
            if (e == null) continue;
            e.MoveToY(y);
            y -= e.Height + entrySpacing;
        }
    }

    void OnDestroy() => _subject.Dispose();
}
