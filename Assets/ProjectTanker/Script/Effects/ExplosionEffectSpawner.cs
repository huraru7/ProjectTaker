using UnityEngine;

/// <summary>
/// ExplosionEffect.Prefab の静的参照を Scene 起動時にセットするブリッジ。
/// 空 GameObject に付けて _prefab に Explosion.prefab をアサインするだけでよい。
/// </summary>
public class ExplosionEffectSpawner : MonoBehaviour
{
    [SerializeField] private ExplosionEffect _prefab;

    void Awake() => ExplosionEffect.Prefab = _prefab;
}
