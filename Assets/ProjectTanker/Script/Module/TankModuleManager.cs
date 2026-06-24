using System.Collections.Generic;
using R3;
using UnityEngine;

public class TankModuleManager : MonoBehaviour
{
    [SerializeField] private TankStatus _tankStatus;

    [Tooltip("存在するモジュール一覧")] [SerializeField] private List<ModuleData> moduleLists;
    [Tooltip("装備slot")] private ModuleData[] slots = new ModuleData[7];
    public IReadOnlyList<ModuleData> Slots => slots;

    [Tooltip("ゲーム全体で共有・消費されるリロール回数")]
    [SerializeField] private int maxRerollCount = 3;
    private int _rerollCount;

    private Dictionary<ModuleData, int> stackCounts = new();

    private readonly Subject<ModuleData[]> _onModuleCandidatesGenerated = new();
    public Observable<ModuleData[]> OnModuleCandidatesGenerated => _onModuleCandidatesGenerated;

    private readonly Subject<IReadOnlyList<ModuleData>> _onSlotsChanged = new();
    public Observable<IReadOnlyList<ModuleData>> OnSlotsChanged => _onSlotsChanged;

    private readonly Subject<int> _onRerollCountChanged = new();
    public Observable<int> OnRerollCountChanged => _onRerollCountChanged;

    // 3択UIが表示中かどうか。表示中に追加でレベルが上がった場合は獲得をキューへ積む
    private bool _isSelectionPending;
    private int  _queuedEarns;

    void Awake()
    {
        _rerollCount = maxRerollCount;
    }

    /// <summary>
    /// 3択候補を生成して Presenter へ通知する。既に選択中なら完了後に表示するためキューへ積む
    /// </summary>
    public void ModuleEarn()
    {
        if (_isSelectionPending)
        {
            _queuedEarns++;
            return;
        }
        GenerateAndEmitCandidates();
    }

    private void GenerateAndEmitCandidates()
    {
        _isSelectionPending = true;

        List<ModuleData> pool = new(moduleLists);
        ModuleData[] candidates = new ModuleData[Mathf.Min(3, pool.Count)];
        for (int i = 0; i < candidates.Length; i++)
        {
            int idx = Random.Range(0, pool.Count);
            candidates[i] = pool[idx];
            pool.RemoveAt(idx);
        }
        _onModuleCandidatesGenerated.OnNext(candidates);
        _onRerollCountChanged.OnNext(_rerollCount);
    }

    /// <summary>
    /// リロール残数を1消費して3択を再抽選する。残数が無ければ何もしない。
    /// </summary>
    public void RerollCandidates()
    {
        if (_rerollCount <= 0) return;
        _rerollCount--;
        GenerateAndEmitCandidates();
    }

    /// <summary>
    /// 3択UIでの選択〜装備（またはスロット入れ替えの決定）が完了した際に Presenter から呼ぶ。
    /// 保留中の獲得があれば次の3択を続けて表示する。
    /// </summary>
    public void NotifySelectionResolved()
    {
        _isSelectionPending = false;
        if (_queuedEarns > 0)
        {
            _queuedEarns--;
            GenerateAndEmitCandidates();
        }
    }

    /// <summary>
    /// 空きスロットに自動装備する。空きがなければ false を返す。
    /// </summary>
    public bool TryAutoEquip(ModuleData module)
    {
        if (module == null) return false;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                SetModule(i, module);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指定スロットのモジュールを更新する
    /// </summary>
    public void SetModule(int slotIndex, ModuleData newModule)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        ModuleData oldModule = slots[slotIndex];
        if (oldModule != null && stackCounts.ContainsKey(oldModule))
        {
            stackCounts[oldModule]--;
            if (stackCounts[oldModule] <= 0)
                stackCounts.Remove(oldModule);
        }

        slots[slotIndex] = newModule;

        if (newModule != null)
        {
            if (!stackCounts.ContainsKey(newModule))
                stackCounts[newModule] = 0;
            stackCounts[newModule]++;
        }

        Debug.Log($"[Slot] スロット{slotIndex} → {newModule?.moduleName ?? "null"}  (前: {oldModule?.moduleName ?? "null"})");

        RecalculateStats();
        _onSlotsChanged.OnNext(slots);
    }

    private void RecalculateStats()
    {
        _tankStatus.ResetStatusWithoutHP();
        foreach (var (module, count) in stackCounts)
        {
            if (count <= 0) continue;
            foreach (var effect in module.specialEffects)
                effect.Apply(_tankStatus, count);
        }
    }

    private void OnDestroy()
    {
        _onModuleCandidatesGenerated.Dispose();
        _onSlotsChanged.Dispose();
        _onRerollCountChanged.Dispose();
    }
}
