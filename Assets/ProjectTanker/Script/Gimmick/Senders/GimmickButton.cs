using UnityEngine;

public class GimmickButton : SignalSender
{
    [SerializeField] private bool sendOnce = false;
    private bool _hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (sendOnce && _hasTriggered) return;

        if (other.TryGetComponent<Bullet>(out _))
        {
            _hasTriggered = true;
            Send(true);
        }
    }
}
