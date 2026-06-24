using R3;
using UnityEngine;

public class TankPresenter : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private TankStatus tankStatus;
    [SerializeField] private TankModuleManager tankModuleManager;

    [Header("View")]
    [SerializeField] private GetModuleSelectUI getModuleSelectUI;
    [SerializeField] private SlotUI slotUI;
    [SerializeField] private ModuleReplaceUI moduleReplaceUI;

    private ModuleData _pendingModule;

    void Start()
    {
        // Model→View: 3択候補が生成されたらUIに表示
        tankModuleManager.OnModuleCandidatesGenerated
            .Subscribe(candidates => getModuleSelectUI.ShowOptions(candidates))
            .AddTo(this);

        // View→Model: 選択されたモジュールを空きスロットへ自動装備、満杯なら入れ替えUIへ
        getModuleSelectUI.OnModuleSelected
            .Subscribe(selected =>
            {
                if (tankModuleManager.TryAutoEquip(selected))
                {
                    _pendingModule = null;
                    tankModuleManager.NotifySelectionResolved();
                }
                else
                {
                    _pendingModule = selected;
                    moduleReplaceUI.Show(selected, tankModuleManager.Slots);
                }
            })
            .AddTo(this);

        // View→Model: 入れ替え/破棄の決定を受け取る
        moduleReplaceUI.OnDecision
            .Subscribe(slotIndex =>
            {
                if (slotIndex >= 0 && _pendingModule != null)
                    tankModuleManager.SetModule(slotIndex, _pendingModule);
                _pendingModule = null;
                tankModuleManager.NotifySelectionResolved();
            })
            .AddTo(this);

        // 初期表示
        slotUI.UpdateDisplay(tankModuleManager.Slots);

        // Model→View: スロットが変化したら一覧を更新
        tankModuleManager.OnSlotsChanged
            .Subscribe(slots => slotUI.UpdateDisplay(slots))
            .AddTo(this);

        // Model→View: リロール残数が変化したら表示更新
        tankModuleManager.OnRerollCountChanged
            .Subscribe(count => getModuleSelectUI.UpdateRerollCount(count))
            .AddTo(this);

        // View→Model: リロール要求を受け取る
        getModuleSelectUI.OnRerollRequested
            .Subscribe(_ => tankModuleManager.RerollCandidates())
            .AddTo(this);
    }
}