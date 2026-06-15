using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

/// <summary>
/// LitMotion の Shake を使ってカメラを揺らす演出。
/// Cinemachine 不要。_cameraTransform の localPosition を一時的にブレさせる。
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private Transform _cameraTransform;

    void Awake()
    {
        Instance = this;
        if (_cameraTransform == null)
            _cameraTransform = Camera.main?.transform;
    }

    /// <summary>カメラを指定した強さ・時間でシェイクする。</summary>
    public void Shake(float strength = 0.3f, float duration = 0.25f)
    {
        if (_cameraTransform == null) return;

        LMotion.Shake.Create(Vector3.zero, new Vector3(strength, strength, 0f), duration)
            .WithFrequency(12)
            .WithDampingRatio(1f)
            .BindToLocalPosition(_cameraTransform)
            .AddTo(this);
    }
}
