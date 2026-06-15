using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [SerializeField] private ExpOrb            orbPrefab;
    [SerializeField] private Transform         playerTransform;
    [SerializeField] private TankModuleManager moduleManager;
    [SerializeField] private int               poolSize = 30;

    private int _currentXp;
    private int _currentLevel = 1;
    private readonly Queue<ExpOrb> _pool = new();

    private readonly Subject<Unit> _onXpChanged = new();
    public Observable<Unit> OnXpChanged => _onXpChanged;

    public int CurrentLevel => _currentLevel;
    public int CurrentXp    => _currentXp;
    public int RequiredXp   => Mathf.CeilToInt(Mathf.Pow(_currentLevel, 1.25f)) + 20;

    private static readonly Dictionary<Type, int> BaseXpTable = new()
    {
        { typeof(CowardAI),       5 },
        { typeof(ChaseAndFireAI), 8 },
        { typeof(SniperAI),      10 },
        { typeof(ExpertAI),      12 },
    };

    void Awake()
    {
        Instance = this;
        InitPool();
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(orbPrefab, transform);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    private ExpOrb GetOrb()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();
        return Instantiate(orbPrefab, transform);
    }

    public void ReturnOrb(ExpOrb orb)
    {
        orb.gameObject.SetActive(false);
        _pool.Enqueue(orb);
    }

    public void DropXp(Vector2 position, EnemyAIBase ai)
    {
        if (ai == null) return;

        int   baseXp  = BaseXpTable.TryGetValue(ai.GetType(), out var v) ? v : 8;
        float scale   = 0.6f + ai.GetDifficulty() * 0.2f; // diff1=0.8 ~ diff5=1.6
        int   totalXp = Mathf.Max(1, Mathf.RoundToInt(baseXp * scale * UnityEngine.Random.Range(0.8f, 1.2f)));

        int orbCount = UnityEngine.Random.Range(3, 6);
        for (int i = 0; i < orbCount; i++)
        {
            if (totalXp <= 0) break;

            int portion = (i == orbCount - 1 || totalXp == 1)
                ? totalXp
                : UnityEngine.Random.Range(1, totalXp);
            totalXp -= portion;

            // 死亡地点を中心に周辺へランダム配置（速度で飛ばすのではなく位置をずらす）
            Vector2 offset = UnityEngine.Random.insideUnitCircle * 0.8f;

            var orb = GetOrb();
            orb.transform.position = position + offset;
            orb.gameObject.SetActive(true);
            orb.Initialize(portion, playerTransform, Vector2.zero);
        }
    }

    public void CollectXp(int amount)
    {
        _currentXp += amount;
        while (_currentXp >= RequiredXp)
        {
            _currentXp -= RequiredXp;
            _currentLevel++;
            moduleManager.ModuleEarn();
        }
        _onXpChanged.OnNext(Unit.Default);
    }

    void OnDestroy() => _onXpChanged.Dispose();
}
