using UnityEngine;

public abstract class SignalReceiver : MonoBehaviour
{
    [SerializeField] private SignalChannel channel;

    void OnEnable()  => channel.OnSignalChanged += OnReceive;
    void OnDisable() => channel.OnSignalChanged -= OnReceive;

    protected abstract void OnReceive(bool value);
}
