using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    public static OffScreenIndicator Instance { get; private set; }

    [SerializeField] private OffScreenIndicatorEntry entryPrefab;
    [SerializeField] private RectTransform           container;
    [SerializeField] private float                   edgePadding = 50f;

    private readonly Dictionary<Transform, OffScreenIndicatorEntry> _entries = new();

    void Awake() => Instance = this;

    /// <summary>ターゲットを追跡リストに追加する。</summary>
    public void Register(Transform target, Color color, string label = "")
    {
        if (target == null || _entries.ContainsKey(target)) return;
        var entry = Instantiate(entryPrefab, container);
        entry.Setup(color, label);
        _entries[target] = entry;
    }

    /// <summary>指定ターゲットの追跡を解除する。</summary>
    public void Unregister(Transform target)
    {
        if (_entries.TryGetValue(target, out var e))
        {
            if (e != null) Destroy(e.gameObject);
            _entries.Remove(target);
        }
    }

    /// <summary>全ターゲットの追跡を解除する。</summary>
    public void UnregisterAll()
    {
        foreach (var e in _entries.Values)
            if (e != null) Destroy(e.gameObject);
        _entries.Clear();
    }

    void LateUpdate()
    {
        var cam = Camera.main;
        if (cam == null) return;

        foreach (var (target, entry) in _entries)
        {
            if (entry == null) continue;

            if (target == null || !target.gameObject.activeInHierarchy)
            {
                entry.gameObject.SetActive(false);
                continue;
            }

            Vector3 vp = cam.WorldToViewportPoint(target.position);

            bool onScreen = vp.z > 0
                         && vp.x > 0.05f && vp.x < 0.95f
                         && vp.y > 0.05f && vp.y < 0.95f;

            if (onScreen)
            {
                entry.gameObject.SetActive(false);
                continue;
            }

            entry.gameObject.SetActive(true);

            // カメラ後方は方向を反転
            if (vp.z < 0) { vp.x = 1 - vp.x; vp.y = 1 - vp.y; }

            var    center    = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var    screenPos = new Vector2(vp.x * Screen.width, vp.y * Screen.height);
            Vector2 dir      = (screenPos - center).normalized;

            float hw = Screen.width  * 0.5f - edgePadding;
            float hh = Screen.height * 0.5f - edgePadding;

            Vector2 edgePos;
            if (Mathf.Abs(dir.x) * hh > Mathf.Abs(dir.y) * hw)
            {
                edgePos.x = dir.x > 0 ? hw : -hw;
                edgePos.y = dir.y * hw / Mathf.Abs(dir.x);
            }
            else
            {
                edgePos.y = dir.y > 0 ? hh : -hh;
                edgePos.x = dir.x * hh / Mathf.Abs(dir.y);
            }

            Vector2 worldScreenPos = center + edgePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                container, worldScreenPos, null, out Vector2 localPos);

            float angle    = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float distance = Vector3.Distance(cam.transform.position, target.position);

            entry.UpdateIndicator(localPos, angle, distance);
        }
    }
}
