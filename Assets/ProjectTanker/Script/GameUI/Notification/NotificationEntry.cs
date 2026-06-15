using System.Collections;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationEntry : MonoBehaviour
{
    [SerializeField] private Image           accentBar;
    [SerializeField] private Image           iconImage;
    [SerializeField] private GameObject      iconRoot;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float           slideInDuration = 0.25f;
    [SerializeField] private float           fadeOutDuration = 0.4f;
    [SerializeField] private float           slideOffsetX    = -60f;

    private RectTransform _rt;
    private CanvasGroup   _cg;
    private MotionHandle  _moveHandle;

    public float Height => _rt.rect.height;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _cg = GetComponent<CanvasGroup>();
    }

    public void Initialize(NotificationData data, float defaultDuration)
    {
        messageText.text = data.Message;
        accentBar.color  = data.AccentColor;
        iconRoot.SetActive(data.Icon != null);
        if (data.Icon != null) iconImage.sprite = data.Icon;

        float dur = data.Duration > 0f ? data.Duration : defaultDuration;
        StartCoroutine(LifecycleCo(dur));
    }

    private IEnumerator LifecycleCo(float duration)
    {
        // 現在位置を終点、左にオフセットした位置を始点としてスライドイン
        float targetX = _rt.anchoredPosition.x;
        float startX  = targetX + slideOffsetX;
        _cg.alpha            = 0f;
        _rt.anchoredPosition = new Vector2(startX, _rt.anchoredPosition.y);

        LMotion.Create(0f, 1f, slideInDuration)
            .WithEase(Ease.OutCubic)
            .Bind(v => _cg.alpha = v)
            .AddTo(this);

        // Y は RearrangeEntries が MoveToY で管理するため X のみアニメーション
        LMotion.Create(startX, targetX, slideInDuration)
            .WithEase(Ease.OutCubic)
            .BindToAnchoredPositionX(_rt)
            .AddTo(this);

        yield return new WaitForSecondsRealtime(slideInDuration + duration);

        // フェードアウト開始後、終了まで待ってから破棄
        LMotion.Create(1f, 0f, fadeOutDuration)
            .WithEase(Ease.InCubic)
            .Bind(v => _cg.alpha = v)
            .AddTo(this);

        yield return new WaitForSecondsRealtime(fadeOutDuration);

        Destroy(gameObject);
    }

    public void MoveToY(float targetY)
    {
        if (_moveHandle.IsActive()) _moveHandle.Cancel();
        _moveHandle = LMotion.Create(_rt.anchoredPosition.y, targetY, 0.2f)
            .WithEase(Ease.OutCubic)
            .BindToAnchoredPositionY(_rt)
            .AddTo(this);
    }

    void OnDestroy()
    {
        NotificationManager.Instance?.OnEntryDestroyed(this);
    }
}
