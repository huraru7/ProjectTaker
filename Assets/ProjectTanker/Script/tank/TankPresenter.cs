using R3;
using UnityEngine;

public class TankPresenter : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private TankStatus tankStatus;
    [SerializeField] private TankModuleManager tankModuleManager;

    [Header("View")]
    [SerializeField] private GetModuleSelectUI getModuleSelectUI;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private SlotUI slotUI;

    void Start()
    {
        //:Model→View: 3択候補が生成されたらUIに表示
        tankModuleManager.OnModuleCandidatesGenerated
            .Subscribe(candidates => getModuleSelectUI.ShowOptions(candidates))
            .AddTo(this);

        //:View→Model: プレイヤーが選択したらインベントリへ追加
        getModuleSelectUI.OnModuleSelected
            .Subscribe(selected => tankModuleManager.AddToInventory(selected))
            .AddTo(this);

        //:Model→View: インベントリが変化したら一覧を更新
        tankModuleManager.OnInventoryChanged
            .Subscribe(inventory => inventoryUI.UpdateDisplay(inventory))
            .AddTo(this);

        //:View→Model: ドラッグドロップでモジュールをスロットへ装備
        slotUI.OnModuleDropped
            .Subscribe(e => tankModuleManager.SetModule(e.slotIndex, e.module))
            .AddTo(this);

        //:View→Model: スロットからインベントリへドラッグドロップで取り外し
        inventoryUI.OnModuleReturnedFromSlot
            .Subscribe(slotIndex => tankModuleManager.RemoveFromSlot(slotIndex))
            .AddTo(this);

        //:初期表示（全スロット空状態）
        slotUI.UpdateDisplay(tankModuleManager.Slots);

        //:Model→View: スロットが変化したら一覧を更新
        tankModuleManager.OnSlotsChanged
            .Subscribe(slots => slotUI.UpdateDisplay(slots))
            .AddTo(this);

        tankModuleManager.ModuleEarn();//:デバッグ用
    }
}
