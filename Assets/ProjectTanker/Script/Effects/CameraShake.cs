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

    // MainCameraFollower が LateUpdate で読み取って最終位置に加算する
    public Vector3 ShakeOffset { get; private set; }

    void Awake() => Instance = this;

    /// <summary>カメラを指定した強さ・時間でシェイクする。</summary>
    public void Shake(float strength = 0.3f, float duration = 0.25f)
    {
        LMotion.Shake.Create(Vector3.zero, new Vector3(strength, strength, 0f), duration)
            .WithFrequency(12)
            .WithDampingRatio(1f)
            .Bind(offset => ShakeOffset = offset)
            .AddTo(this);
    }
}
