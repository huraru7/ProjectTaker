using UnityEngine;

public class SwitchPart : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Bullet>(out _)) return;
        GetComponentInParent<GimmickButton>()?.TriggerSwitch();
    }
}
