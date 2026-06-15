using UnityEngine;

public abstract class SignalReceiver : MonoBehaviour
{
    [Header("回路接続")]
    [SerializeField] private SignalChannel channel;

    public SignalChannel Channel => channel;

    void OnEnable()  => channel.OnSignalChanged += OnReceive;
    void OnDisable() => channel.OnSignalChanged -= OnReceive;

    protected abstract void OnReceive(bool value);
}
