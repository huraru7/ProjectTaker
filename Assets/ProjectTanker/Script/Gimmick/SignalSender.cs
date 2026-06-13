using UnityEngine;

public abstract class SignalSender : MonoBehaviour
{
    [SerializeField] protected SignalChannel channel;

    protected void Send(bool value) => channel?.Send(value);
}
