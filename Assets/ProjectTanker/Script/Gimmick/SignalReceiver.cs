using UnityEngine;

public abstract class SignalReceiver : MonoBehaviour
{
    [Header("回路接続")]
    [SerializeField] private SignalChannel channel;

    public SignalChannel Channel => channel;

    void OnEnable()
    {
        if (channel == null)
        {
            Debug.LogWarning($"[SignalReceiver] {name} の channel が未設定です。インスペクタで紐付けてください。", this);
            return;
        }
        channel.OnSignalChanged += OnReceive;
    }

    void OnDisable()
    {
        if (channel == null) return;
        channel.OnSignalChanged -= OnReceive;
    }

    protected abstract void OnReceive(bool value);
}
