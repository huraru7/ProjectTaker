using System.Collections;
using LitMotion;
using UnityEngine;

public class GimmickButton : SignalSender
{
    [Header("ビジュアル")]
    [SerializeField] private Transform      buttonTop;
    [SerializeField] private SpriteRenderer topRenderer;
    [SerializeField] private Color          offColor = new Color(0.85f, 0.65f, 0.15f);
    [SerializeField] private Color          onColor  = new Color(0.25f, 0.85f, 0.35f);

    [Header("動作")]
    [Tooltip("0 = 永続ON / 0より大きい値 = 指定秒後にOFFへ戻る")]
    [SerializeField] private float resetDelay  = 0f;
    [SerializeField] private float pressDepth  = 0.15f;
    [SerializeField] private float pressTime   = 0.07f;
    [SerializeField] private float releaseTime = 0.15f;

    private bool         _isOn;
    private MotionHandle _pressHandle;
    private Coroutine    _resetCo;
    private float        _topRestY;

    void Awake()
    {
        if (buttonTop   != null) _topRestY = buttonTop.localPosition.y;
        if (topRenderer != null) topRenderer.color = offColor;
    }

    public void TriggerSwitch()
    {
        if (resetDelay <= 0f && _isOn) return;

        _isOn = true;
        Send(true);
        AnimatePress(-pressDepth, pressTime, Ease.OutCubic);
        if (topRenderer != null) topRenderer.color = onColor;

        if (resetDelay > 0f)
        {
            if (_resetCo != null) StopCoroutine(_resetCo);
            _resetCo = StartCoroutine(ResetCo());
        }
    }

    private IEnumerator ResetCo()
    {
        yield return new WaitForSeconds(resetDelay);
        _isOn = false;
        Send(false);
        AnimatePress(0f, releaseTime, Ease.OutBack);
        if (topRenderer != null) topRenderer.color = offColor;
        _resetCo = null;
    }

    private void AnimatePress(float targetOffsetY, float duration, Ease ease)
    {
        if (_pressHandle.IsActive()) _pressHandle.Cancel();
        float fromY = buttonTop != null ? buttonTop.localPosition.y : _topRestY;
        float toY   = _topRestY + targetOffsetY;
        _pressHandle = LMotion.Create(fromY, toY, duration)
            .WithEase(ease)
            .Bind(y => { if (buttonTop != null) buttonTop.localPosition = new Vector3(0f, y, 0f); })
            .AddTo(this);
    }
}
