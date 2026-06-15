using LitMotion;
using UnityEngine;

public class GimmickButton : SignalSender
{
    [SerializeField] private bool            sendOnce         = false;
    [SerializeField] private Transform       leverTransform;
    [SerializeField] private SpriteRenderer  indicatorRenderer;
    [SerializeField] private float           leverOnAngle     = -45f;
    [SerializeField] private float           animDuration     = 0.2f;

    private bool         _isOn;
    private MotionHandle _leverHandle;

    private static readonly int ShaderIsOn = Shader.PropertyToID("_IsOn");

    public void TriggerSwitch()
    {
        if (sendOnce && _isOn) return;
        _isOn = true;
        Send(true);
        PlayOnAnimation();
    }

    private void PlayOnAnimation()
    {
        if (_leverHandle.IsActive()) _leverHandle.Cancel();

        if (leverTransform != null)
        {
            float fromZ = leverTransform.localEulerAngles.z;
            _leverHandle = LMotion.Create(fromZ, leverOnAngle, animDuration)
                .WithEase(Ease.OutBack)
                .Bind(z => leverTransform.localEulerAngles = new Vector3(0f, 0f, z))
                .AddTo(this);
        }

        if (indicatorRenderer != null)
            indicatorRenderer.material.SetFloat(ShaderIsOn, 1f);
    }
}
