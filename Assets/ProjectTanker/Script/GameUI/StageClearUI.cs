using System.Collections;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

public class StageClearUI : MonoBehaviour
{
    [Header("パネル")]
    [SerializeField] private GameObject  _panel;
    [SerializeField] private CanvasGroup _overlayGroup;

    [Header("テキスト")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _shotsText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _distanceText;

    [Header("ボタン")]
    [SerializeField] private CanvasGroup _buttonGroup;

    void Awake()
    {
        if (_panel != null) _panel.SetActive(false);
    }

    public void Show(int shots, float seconds, float distance)
    {
        if (_panel == null)
        {
            Debug.LogWarning("[StageClearUI] _panel が未設定です。インスペクタで紐付けてください。");
            return;
        }
        gameObject.SetActive(true);
        _panel.SetActive(true);
        StartCoroutine(PlayAnimation(shots, seconds, distance));
    }

    private IEnumerator PlayAnimation(int shots, float seconds, float distance)
    {
        // オーバーレイをフェードイン
        if (_overlayGroup != null)
        {
            _overlayGroup.alpha = 0f;
            LMotion.Create(0f, 1f, 0.3f)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .Bind(a => _overlayGroup.alpha = a)
                .AddTo(this);
            yield return new WaitForSecondsRealtime(0.2f);
        }

        // タイトル：大きく出現してから収まる
        if (_titleText != null)
        {
            _titleText.transform.localScale = Vector3.zero;

            LMotion.Create(0f, 1.4f, 0.35f)
                .WithEase(Ease.OutCubic)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .Bind(s => _titleText.transform.localScale = Vector3.one * s)
                .AddTo(this);

            yield return new WaitForSecondsRealtime(0.35f);

            LMotion.Create(1.4f, 1f, 0.2f)
                .WithEase(Ease.OutQuad)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .Bind(s => _titleText.transform.localScale = Vector3.one * s)
                .AddTo(this);

            yield return new WaitForSecondsRealtime(0.28f);
        }

        // 各テキストをスタガーでスライドイン＋カウントアップ
        if (_shotsText    != null) PlayStatSlideIn(_shotsText,    shots,    "射撃数 :  {0:0} 発");
        yield return new WaitForSecondsRealtime(0.15f);
        if (_timeText     != null) PlayStatSlideIn(_timeText,     seconds,  "クリア時間 :  {0:F1} 秒");
        yield return new WaitForSecondsRealtime(0.15f);
        if (_distanceText != null) PlayStatSlideIn(_distanceText, distance, "移動距離 :  {0:F1} m");
        yield return new WaitForSecondsRealtime(0.45f);

        // ボタンをスケール＋フェードでポップイン
        if (_buttonGroup != null)
        {
            _buttonGroup.alpha                    = 0f;
            _buttonGroup.interactable             = false;
            _buttonGroup.blocksRaycasts           = false;
            _buttonGroup.transform.localScale     = Vector3.one * 0.75f;

            LMotion.Create(0f, 1f, 0.35f)
                .WithEase(Ease.OutBack)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .Bind(t =>
                {
                    _buttonGroup.alpha                = Mathf.Clamp01(t * 1.5f);
                    _buttonGroup.transform.localScale = Vector3.one * Mathf.Lerp(0.75f, 1f, t);
                })
                .AddTo(this);

            yield return new WaitForSecondsRealtime(0.35f);
            _buttonGroup.interactable   = true;
            _buttonGroup.blocksRaycasts = true;
        }
    }

    // スライドイン + フェードイン + カウントアップ を同時に再生
    private void PlayStatSlideIn(TextMeshProUGUI text, float targetValue, string format)
    {
        text.text = string.Format(format, 0f);

        var rect    = text.rectTransform;
        var targetX = rect.anchoredPosition.x;
        var startX  = targetX - 80f;
        rect.anchoredPosition = new Vector2(startX, rect.anchoredPosition.y);

        // スライドイン
        LMotion.Create(startX, targetX, 0.45f)
            .WithEase(Ease.OutCubic)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .BindToAnchoredPositionX(rect)
            .AddTo(this);

        // フェードイン
        var c = text.color;
        text.color = new Color(c.r, c.g, c.b, 0f);
        LMotion.Create(0f, 1f, 0.35f)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .Bind(a => text.color = new Color(c.r, c.g, c.b, a))
            .AddTo(this);

        // カウントアップ
        LMotion.Create(0f, targetValue, 0.6f)
            .WithEase(Ease.OutCubic)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .Bind(v => text.text = string.Format(format, v))
            .AddTo(this);
    }

    public void OnNextStage() { }
}
