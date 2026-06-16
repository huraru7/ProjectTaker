# Unity エディタ作業ガイド — project Tanker

---

## モジュールシステム初期セットアップ

スクリプトはすべて実装済み。以下の手順で ScriptableObject アセット・Prefab を作成し、
TankModuleManager に登録することで動作する。

---

## ① SpecialEffect アセットの作成（8種）

`Project ウィンドウ → 右クリック → Create > Data/Effects/XXX` で各アセットを作成する。

| 作成メニュー | ファイル名（推奨） | 変更するパラメータ |
|---|---|---|
| `Data/Effects/AttackBoostEffect` | `AttackBoostEffect.asset` | Attack Bonus（デフォルト 5） |
| `Data/Effects/ArmorEffect` | `ArmorEffect.asset` | Hp Bonus（デフォルト 20） |
| `Data/Effects/MobilityEffect` | `MobilityEffect.asset` | Speed/Turn Bonus（デフォルト 1/20） |
| `Data/Effects/AmmoEffect` | `AmmoEffect.asset` | Mag Bonus / Reload Reduction（デフォルト 1/0.3） |
| `Data/Effects/FireTrailEffect` | `FireTrailEffect.asset` | Flame Zone Prefab をセット、他はデフォルト可 |
| `Data/Effects/ChainLightningEffect` | `ChainLightningEffect.asset` | Chain Bullet Prefab をセット、他はデフォルト可 |
| `Data/Effects/SplitShotEffect` | `SplitShotEffect.asset` | Mini Prefab をセット、他はデフォルト可 |
| `Data/Effects/BounceAmpEffect` | `BounceAmpEffect.asset` | Bonus Per Bounce（デフォルト 3） |

> **注意**: FireTrail / ChainLightning / SplitShot は Prefab のセットが必要（手順③参照）。

---

## ② Prefab の作成（3種）

### FlameZone Prefab

```
FlameZone (GameObject)
├── FlameZone.cs
├── CircleCollider2D → Is Trigger = TRUE, Radius = 0.4
└── SpriteRenderer（炎スプライト、任意）
```

作成後、`FireTrailEffect.asset` の **Flame Zone Prefab** フィールドにドラッグ。

### MiniBullet Prefab

```
MiniBullet (GameObject)
├── MiniBullet.cs
├── Rigidbody2D → Gravity Scale = 0, Collision Detection = Continuous
├── CircleCollider2D → Radius = 0.1
└── SpriteRenderer（小さな丸スプライト）
```

作成後、`SplitShotEffect.asset` の **Mini Prefab** フィールドにドラッグ。

### ChainBullet Prefab

```
ChainBullet (GameObject)
├── ChainBullet.cs
└── SpriteRenderer（小さな光点スプライト）
```

作成後、`ChainLightningEffect.asset` の **Chain Bullet Prefab** フィールドにドラッグ。

---

## ③ ModuleData アセットの作成（8種）

`Project ウィンドウ → 右クリック → Create > Data/Create ModuleData` で各アセットを作成する。

| Asset 名（推奨） | Module Name | Element | Special Effects にセット |
|---|---|---|---|
| `AttackModule.asset` | 強化弾頭 | Fire | AttackBoostEffect.asset |
| `ArmorModule.asset` | 鉄壁 | Earth | ArmorEffect.asset |
| `MobilityModule.asset` | 機動制御 | Wind | MobilityEffect.asset |
| `AmmoModule.asset` | 速射機構 | Water | AmmoEffect.asset |
| `FlameTrailModule.asset` | フレイムトレイル | Fire | FireTrailEffect.asset |
| `ChainLightningModule.asset` | 連鎖電撃 | Wind | ChainLightningEffect.asset |
| `SplitShotModule.asset` | 分裂弾頭 | None | SplitShotEffect.asset |
| `BounceAmpModule.asset` | バウンスアンプ | Earth | BounceAmpEffect.asset |

---

## ④ TankModuleManager への登録

1. Hierarchy でプレイヤータンクの GameObject を選択
2. `TankModuleManager` コンポーネントの **Module Lists** リストを開く
3. 上記 8 つの ModuleData アセットをすべて追加する

---

## 動作確認

| 操作 | 期待する結果 |
|---|---|
| レベルアップ（経験値取得） | 3択のモジュール選択 UI が表示される |
| 強化弾頭を選択 | 弾のダメージが +5 される |
| 鉄壁を選択 | 最大 HP が +20 される |
| 機動制御を選択 | 移動・旋回速度が上がる |
| 速射機構を選択 | マガジン容量 +1、リロードが速くなる |
| フレイムトレイルを選択 | 弾の後ろに炎ゾーンが出現し、触れた敵にダメージ |
| 連鎖電撃を選択 | 敵を倒したとき、近くの敵へ電撃弾が自動追尾する |
| 分裂弾頭を選択 | 弾が壁に当たった瞬間、ミニ弾 3 発がランダム方向へ飛ぶ |
| バウンスアンプを選択 | 壁反射ごとに弾がオレンジ→赤に変色・拡大し、敵へのダメージが増える |
| 同じモジュールを 2 つ装備 | 効果が 2 倍になる（stack=2） |
