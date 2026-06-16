# モジュール追加ガイド — project Tanker

モジュールには2種類ある。

| 種別 | 内容 | 実装量 |
|---|---|---|
| **基礎モジュール** | TankStatus の値を加算するだけ | SpecialEffect × 1 |
| **特殊モジュール** | 弾の挙動を変える独自ロジック | SpecialEffect + Controller + Behavior（+ Prefab） |

---

## アーキテクチャ

```
ModuleData（ScriptableObject）
 └─ specialEffects: List<SpecialEffect>   ← 複数セット可能
       └─ SpecialEffect（ScriptableObject・抽象）
             Apply(TankStatus, stackCount)   ← レベルアップ/リセット時に呼ばれる
             Remove(TankStatus)             ← 現状未使用（RecalculateStats で代替）
```

```
TankModuleManager.RecalculateStats()
  → TankStatus.ResetStatusWithoutHP()        ← HP 以外を初期値に戻す
  → 各 ModuleData の specialEffects.Apply()  ← stackCount 付きで全効果を再適用
```

同じ ModuleData が複数スロットに入ると `stackCount` が増加し、`Apply()` には合算値が渡される。

---

## 基礎モジュールの追加手順

ステータス値を増減するだけのモジュール（例: HP+、速度+）。

### Step 1: SpecialEffect スクリプトを作成

`Assets/ProjectTanker/Script/Module/Effects/` に新しい .cs を作成。

```csharp
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effects/XXXEffect")]
public class XXXEffect : SpecialEffect
{
    [SerializeField] private int bonus = 10;

    public override void Apply(TankStatus status, int stackCount)
    {
        status.getXxx.Value += bonus * stackCount;
    }

    public override void Remove(TankStatus status) { }
}
```

`Remove()` は空で良い（RecalculateStats が自動でリセットしてから Apply を呼ぶため）。

### Step 2: SpecialEffect アセットを作成

```
Project → 右クリック → Create > Data/Effects/XXXEffect
```

Inspector でパラメータを設定する。

### Step 3: ModuleData アセットを作成

```
Project → 右クリック → Create > Data/Create ModuleData
```

| フィールド | 内容 |
|---|---|
| Module Name | 表示名（例: 強化装甲） |
| Description | 説明文 |
| Icon | UI に表示するアイコン Sprite |
| Module Element | None / Earth / Water / Fire / Wind |
| Special Effects | 手順 2 で作った XXXEffect.asset を追加 |

### Step 4: TankModuleManager に登録

Hierarchy でプレイヤータンクを選択 → `TankModuleManager` の **Module Lists** に ModuleData を追加。

---

## 特殊モジュールの追加手順

弾の挙動を変えるモジュール（FlameTrail/ChainLightning/SplitShot/BounceAmp が参考例）。

### アーキテクチャパターン

```
XXXEffect.Apply()
  └─ TankにXXXController（MonoBehaviour）を GetComponent or AddComponent
       └─ TankBulletManager.OnBulletFired を購読（1回だけ）
            └─ 弾が発射されるたびに XXXBehavior を弾に AddComponent
                 └─ Bullet のイベントを購読して効果を発動
                      ├─ OnWallBounce → 壁反射時の処理
                      ├─ OnImpact(pos, targetStatus) → 敵命中時の処理
                      └─ （または Update/Coroutine で継続動作）
```

### Bullet に用意されているイベント

| イベント | 発火タイミング |
|---|---|
| `OnWallBounce` | 壁に反射した直後 |
| `OnImpact(Vector2, TankStatus)` | 敵に命中し TakeDamage した直後（targetStatus は命中した敵） |

`targetStatus.getHP.Value <= 0` で命中時に敵が死んだか判定できる（OnImpact は TakeDamage の後に発火する）。

`BonusDamage` プロパティで追加ダメージを付与できる（BounceAmp が使用）。プール返却時に自動でリセットされる。

### Step 1: Behavior スクリプトを作成（弾にアタッチ）

```csharp
public class XXXBehavior : MonoBehaviour
{
    private bool _active;

    public void Initialize(/* 必要なパラメータ */)
    {
        _active = true;
        // Bullet イベントを購読
        GetComponent<Bullet>().OnWallBounce += OnBounce;
    }

    void OnDisable()
    {
        _active = false;
        var b = GetComponent<Bullet>();
        if (b != null) b.OnWallBounce -= OnBounce;
    }

    private void OnBounce()
    {
        if (!_active) return;
        // 効果を実装
    }
}
```

`OnDisable()` で必ずイベント購読を解除すること（弾はプールで使い回されるため）。

### Step 2: Controller スクリプトを作成（タンクにアタッチ）

```csharp
public class XXXController : MonoBehaviour
{
    private TankBulletManager _bm;
    // 効果パラメータをフィールドで保持

    public void Setup(/* パラメータ */)
    {
        // パラメータを保存
        if (_bm == null)   // 重複購読防止
        {
            _bm = GetComponent<TankBulletManager>();
            if (_bm != null) _bm.OnBulletFired += HandleBulletFired;
        }
    }

    void OnDestroy()
    {
        if (_bm != null) _bm.OnBulletFired -= HandleBulletFired;
    }

    private void HandleBulletFired(Bullet bullet)
    {
        var existing = bullet.GetComponent<XXXBehavior>();
        if (existing != null) Destroy(existing);   // 前の Behavior を削除
        bullet.gameObject.AddComponent<XXXBehavior>().Initialize(/* パラメータ */);
    }
}
```

`_bm == null` チェックで `RecalculateStats()` が複数回呼ばれても重複購読しない。

### Step 3: SpecialEffect スクリプトを作成

```csharp
[CreateAssetMenu(menuName = "Data/Effects/XXXEffect")]
public class XXXEffect : SpecialEffect
{
    [SerializeField] private int someParam = 5;

    public override void Apply(TankStatus status, int stackCount)
    {
        var ctrl = status.GetComponent<XXXController>();
        if (ctrl == null) ctrl = status.gameObject.AddComponent<XXXController>();
        ctrl.Setup(someParam * stackCount);
    }

    public override void Remove(TankStatus status)
    {
        var ctrl = status.GetComponent<XXXController>();
        if (ctrl != null) Destroy(ctrl);
    }
}
```

### Step 4 以降

基礎モジュールの Step 2〜4 と同じ手順で、アセット作成・ModuleData 登録・TankModuleManager への追加を行う。

Prefab が必要な場合（FlameZone, MiniBullet, ChainBullet など）は先に作成して SpecialEffect アセットの Inspector にセットする。

---

## スタック時の効果設計指針

| パターン | 実装方法 |
|---|---|
| 数値が増える（基礎モジュール） | `bonus * stackCount` を加算 |
| 発射数が増える（SplitShot） | `baseMiniCount + (stackCount - 1)` |
| 継続回数が増える（ChainLightning） | `chainCount * stackCount` |
| 1回あたりの効果が増える（BounceAmp） | `bonusPerBounce * stackCount` を Setup に渡す |

---

## スクリプトの格納場所

```
Assets/ProjectTanker/Script/Module/
├── SpecialEffect.cs              （ScriptableObject 抽象クラス）
├── ModuleData.cs                 （モジュール定義 ScriptableObject）
├── TankModuleManager.cs          （スロット管理・RecalculateStats）
└── Effects/
    ├── AttackBoostEffect.cs      （基礎: 攻撃力+）
    ├── ArmorEffect.cs            （基礎: 最大HP+）
    ├── MobilityEffect.cs         （基礎: 移動・旋回速度+）
    ├── AmmoEffect.cs             （基礎: 弾数・リロード改善）
    ├── FireTrailEffect.cs        （特殊: 炎ゾーン）
    ├── FireTrailController.cs
    ├── FireTrailBehavior.cs
    ├── FlameZone.cs
    ├── ChainLightningEffect.cs   （特殊: キル連鎖）
    ├── ChainLightningController.cs
    ├── ChainLightningBehavior.cs
    ├── ChainBullet.cs
    ├── SplitShotEffect.cs        （特殊: 壁反射で分裂）
    ├── SplitShotController.cs
    ├── SplitShotBehavior.cs
    ├── MiniBullet.cs
    ├── BounceAmpEffect.cs        （特殊: 反射でダメージ増加）
    ├── BounceAmpController.cs
    └── BounceAmpBehavior.cs
```
