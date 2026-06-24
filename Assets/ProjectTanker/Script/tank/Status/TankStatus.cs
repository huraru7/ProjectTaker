using R3;
using UnityEngine;

public class TankStatus : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TankData data;

    [Header("Status")]
    [SerializeField] private SerializableReactiveProperty<int> HP;
    [SerializeField] private SerializableReactiveProperty<int> maxHP;
    [SerializeField] private SerializableReactiveProperty<int> baseAttackPower;
    [SerializeField] private SerializableReactiveProperty<int> movementSpeed;
    [SerializeField] private SerializableReactiveProperty<int> turnRate;
    [SerializeField] private SerializableReactiveProperty<float> bulletSpeed;
    [SerializeField] private SerializableReactiveProperty<int> magazineCapacity;
    [SerializeField] private SerializableReactiveProperty<float> reloadTime;
    [SerializeField] private SerializableReactiveProperty<float> barrelTurnRate;
    [SerializeField] private SerializableReactiveProperty<float> cameraViewSize;

    private Subject<Unit> _onDead = new();
    public Observable<Unit> OnDead => _onDead;

    public SerializableReactiveProperty<int> getHP => HP;
    public SerializableReactiveProperty<int> getMaxHP => maxHP;
    public SerializableReactiveProperty<int> getBaseAttackPower => baseAttackPower;
    public SerializableReactiveProperty<int> getMagazineCapacity => magazineCapacity;
    public SerializableReactiveProperty<int> getMovementSpeed => movementSpeed;
    public SerializableReactiveProperty<int> getTurnRate => turnRate;
    public SerializableReactiveProperty<float> getBulletSpeed => bulletSpeed;
    public SerializableReactiveProperty<float> getReloadTime => reloadTime;
    public SerializableReactiveProperty<float> getBarrelTurnRate => barrelTurnRate;
    public SerializableReactiveProperty<float> getCameraViewSize => cameraViewSize;

    void Awake()
    {
        if (data == null)
        {
            Debug.LogError($"TankDataが割り当てられていません。", this);
            return;
        }

        HP = new(data.maxHP);
        maxHP = new(data.maxHP);
        baseAttackPower = new(data.baseAttackPower);
        movementSpeed = new(data.movementSpeed);
        turnRate = new(data.turnRate);
        bulletSpeed = new(data.bulletSpeed);
        magazineCapacity = new(data.magazineCapacity);
        reloadTime = new(data.reloadTime);
        barrelTurnRate = new(data.barrelTurnRate);
        cameraViewSize = new(data.cameraViewSize);
    }

    /// <summary>
    /// ステータスのリセット処理(モジュール再計算時に呼び出す)
    /// </summary>
    public void ResetStatus()
    {
        HP.Value = data.maxHP;
        maxHP.Value = data.maxHP;
        baseAttackPower.Value = data.baseAttackPower;
        movementSpeed.Value = data.movementSpeed;
        turnRate.Value = data.turnRate;
        bulletSpeed.Value = data.bulletSpeed;
        magazineCapacity.Value = data.magazineCapacity;
        reloadTime.Value = data.reloadTime;
        barrelTurnRate.Value = data.barrelTurnRate;
        cameraViewSize.Value = data.cameraViewSize;
    }

    /// <summary>
    /// 現在HPを維持したままステータスをリセットする（モジュール再計算用）
    /// </summary>
    public void ResetStatusWithoutHP()
    {
        maxHP.Value = data.maxHP;
        baseAttackPower.Value = data.baseAttackPower;
        movementSpeed.Value = data.movementSpeed;
        turnRate.Value = data.turnRate;
        bulletSpeed.Value = data.bulletSpeed;
        magazineCapacity.Value = data.magazineCapacity;
        reloadTime.Value = data.reloadTime;
        barrelTurnRate.Value = data.barrelTurnRate;
        cameraViewSize.Value = data.cameraViewSize;
    }

    public void DealDamage(int amount)
    {
        if (amount <= 0) return;
        HP.Value = Mathf.Clamp(HP.Value - amount, 0, maxHP.Value);
        if (HP.Value == 0) _onDead.OnNext(Unit.Default);
    }
}