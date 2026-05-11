using System.Collections.Generic;
using R3;
using UnityEngine;

public class TankModuleManager : MonoBehaviour
{
    //:モジュール機能の管理　tank自体につける
    //do:モジュールの獲得・消去　モジュール一覧 インベントリ モジュールのセット(インベントリからの移動)
    [SerializeField] private TankStatus _tankStatus;

    [Tooltip("存在するモジュール一覧")][SerializeField] private List<ModuleData> moduleLists;
    [Tooltip("所持モジュールインベントリ")] public List<ModuleData> moduleInventory { get; private set; } = new List<ModuleData>();
    [Tooltip("装備slot")] private ModuleData[] slots = new ModuleData[7];

    /// <summary>
    /// 持っているモジュールの種類と個数を記録する辞書型のデータ
    /// </summary>
    private Dictionary<ModuleData, int> stackCounts = new();

    private readonly Subject<ModuleData[]> _onModuleCandidatesGenerated = new();
    public Observable<ModuleData[]> OnModuleCandidatesGenerated => _onModuleCandidatesGenerated;

    private readonly Subject<IReadOnlyList<ModuleData>> _onInventoryChanged = new();
    public Observable<IReadOnlyList<ModuleData>> OnInventoryChanged => _onInventoryChanged;

    public void Start()
    {
        ModuleEarn();//:デバッグ
    }
    /// <summary>
    /// 3択候補を生成してPresenterへ通知する
    /// </summary>
    public void ModuleEarn()
    {
        //do:重みつきランダムへの改良を検討(ゲームバランス的に)
        List<ModuleData> pool = new(moduleLists);
        ModuleData[] candidates = new ModuleData[Mathf.Min(3, pool.Count)];
        for (int i = 0; i < candidates.Length; i++)
        {
            int idx = Random.Range(0, pool.Count);
            candidates[i] = pool[idx];
            pool.RemoveAt(idx);
        }
        _onModuleCandidatesGenerated.OnNext(candidates);
    }

    /// <summary>
    /// プレイヤーが選んだモジュールをインベントリへ追加
    /// </summary>
    public void AddToInventory(ModuleData selected)
    {
        if (selected == null) return;
        moduleInventory.Add(selected);
        _onInventoryChanged.OnNext(moduleInventory); //:インベントリ変化を通知
    }

    private void OnDestroy()
    {
        _onModuleCandidatesGenerated.Dispose();
        _onInventoryChanged.Dispose();
    }

    /// <summary>
    /// 装備欄のモジュールを更新します
    /// </summary>
    /// <param name="slotIndex">モジュールを入れるスロットのインデックス</param>
    /// <param name="newModule">入るモジュールデータ</param>
    public void SetModule(int slotIndex, ModuleData newModule)
    {
        //:slotIndexの範囲外なら処理しない
        if (slotIndex < 0 || slotIndex >= slots.Length)
            return;

        //:slotsを更新
        //:入れる予定のスロットにあるモジュールを見て、入っていたら古いモジュールを減らす
        ModuleData oldModule = slots[slotIndex];
        if (oldModule != null)
        {
            stackCounts[oldModule]--;
        }

        //:新しいモジュールに置き換える
        slots[slotIndex] = newModule;

        //:新しいモジュールがnullでなければ新しいモジュールのカウントを増やす
        if (newModule != null)
        {
            if (!stackCounts.ContainsKey(newModule))
            {
                //:初めて入るモジュールの場合0で初期化した後にカウントを足す
                stackCounts[newModule] = 0;
            }
            stackCounts[newModule]++;
        }

        //:効果の再計算を行う
        RecalculateStats();
    }

    /// <summary>
    /// 効果の内部計算を行います
    /// </summary>
    private void RecalculateStats()
    {
        //!:再計算する前にstatusを初期値に戻してから計算してください
        _tankStatus.ResetStatus();

        foreach (var (module, count) in stackCounts)
        {
            //:nullなら計算せずに次へ
            if (count <= 0) continue;

            foreach (var effect in module.specialEffects)
            {
                //:モジュール効果が各ステータスを更新する
                effect.Apply(_tankStatus, count);
            }
        }
    }
}
