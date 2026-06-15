using UnityEngine;

public abstract class SignalSender : MonoBehaviour
{
    [Header("回路接続")]
    [SerializeField] protected SignalChannel channel;

    protected void Send(bool value) => channel?.Send(value);

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (channel == null) return;
        var color = ChannelGizmoColor(channel);

        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, 0.18f);

        foreach (var r in FindObjectsByType<SignalReceiver>(FindObjectsSortMode.None))
        {
            if (r.Channel != channel) continue;
            Gizmos.color = color;
            Gizmos.DrawLine(transform.position, r.transform.position);
            Gizmos.DrawWireSphere(r.transform.position, 0.18f);
        }
    }

    internal static Color ChannelGizmoColor(SignalChannel ch)
    {
        int h = ch.GetHashCode();
        return Color.HSVToRGB(((h >> 8) & 0xFF) / 255f, 0.8f, 0.95f);
    }
#endif
}
