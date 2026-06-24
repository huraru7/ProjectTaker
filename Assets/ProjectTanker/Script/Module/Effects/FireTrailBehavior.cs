using System.Collections;
using UnityEngine;

public class FireTrailBehavior : MonoBehaviour
{
    private bool _active;

    public void Initialize(GameObject prefab, float interval, float duration, int damage, TankStatus owner)
    {
        _active = true;
        StartCoroutine(SpawnCo(prefab, interval, duration, damage, owner));
    }

    void OnDisable()
    {
        _active = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnCo(GameObject prefab, float interval, float duration, int damage, TankStatus owner)
    {
        var wait = new WaitForSeconds(interval);
        while (_active)
        {
            var zone = Instantiate(prefab, transform.position, Quaternion.identity);
            zone.GetComponent<FlameZone>().Initialize(duration, damage, owner);
            yield return wait;
        }
    }
}
