# Unity エディタ作業ガイド — Enemy System セットアップ

> コード側の変更（スクリプト実装）が完了してから、このガイドに沿って Unity エディタ側を整理してください。

---

## カラーリファレンス（コピー用）

```
背景         #F0EDE8
パネル       #E9E5DF
枠線         #CFC9C2
テキスト主   #3D3833
テキスト副   #7A766F

Earth        #7CB87E
Water        #7BAFD4
Fire         #E07A5F
Wind         #F2CC6A
None         #B0ABA3
```

---

# STEP 1 — Enemy 用 TankData ScriptableObject の作成

## 1-1. TankData アセットを複製する

1. Project ウィンドウで `Assets/ProjectTanker/Data/TankData.asset` を選択
2. `Ctrl+D` で複製 → 名前を **`EnemyTankData`** に変更
3. Inspector を開き、適宜ステータスを調整する（HP、移動速度など）

> プレイヤーと同じ値でも構いません。後から調整可能です。

---

# STEP 2 — Enemy GameObject の作成

## 2-1. 既存プレイヤー Prefab を参考に Enemy を作る

1. Hierarchy でプレイヤーの Tank オブジェクト（Rigidbody2D、TankStatus が付いているもの）を確認する
2. Hierarchy を右クリック → **`Create Empty`**、名前を **`EnemyTank`** に変更
3. プレイヤーと同じ見た目の Sprite（SpriteRenderer）を子オブジェクトとして追加する（色を変えて識別しやすくする）

---

## 2-2. 必要なコンポーネントをアタッチする

`EnemyTank` オブジェクトを選択し、`Add Component` から以下を順番に追加する。

| コンポーネント | 説明 |
|---|---|
| `Rigidbody2D` | 物理移動に必要。Gravity Scale を **0** に設定（2Dトップダウン） |
| `Collider2D`（Polygon または Circle） | 衝突判定。プレイヤーと同じ形状を推奨 |
| `TankStatus` | ステータス管理（プレイヤーと共用） |
| `EnemyBulletManager` | 弾の発射・プール管理（Enemy専用） |
| `EnemyMovement` | AI命令を受けて移動する |
| `EnemyManager` | AI戦略とコンポーネントの橋渡し |

---

## 2-3. TankStatus の Inspector をアサイン

`EnemyTank` の `TankStatus` コンポーネントに以下をアサイン:

| フィールド | アサインするもの |
|---|---|
| `Data` | `EnemyTankData`（STEP 1 で作成したもの） |

---

## 2-4. EnemyBulletManager の Inspector をアサイン

`EnemyBulletManager` コンポーネントに以下をアサイン:

| フィールド | アサインするもの |
|---|---|
| `Tank Status` | `EnemyTank` の `TankStatus` コンポーネント |
| `Bullet` | 弾の Prefab（プレイヤーと同じもので可） |
| `Bullet Parent` | 弾を格納する空 GameObject（`BulletPool` などの名前を付けて作成） |
| `Pool Size` | 5（デフォルト） |
| `Is Shoot` | ✅ ON |

> `BulletPool` オブジェクトは Hierarchy に空の GameObject として作成し、`EnemyTank` の子にしておくと管理しやすい。

---

## 2-5. EnemyMovement の Inspector をアサイン

| フィールド | アサインするもの |
|---|---|
| `Tank Status` | `EnemyTank` の `TankStatus` コンポーネント |

> `Rigidbody2D` は `[RequireComponent]` によって自動で取得されます。

---

## 2-6. EnemyManager の Inspector をアサイン

| フィールド | アサインするもの |
|---|---|
| `Tank Status` | `EnemyTank` の `TankStatus` コンポーネント |
| `Movement` | `EnemyTank` の `EnemyMovement` コンポーネント |
| `Bullet Manager` | `EnemyTank` の `EnemyBulletManager` コンポーネント |
| `Player Transform` | シーン内のプレイヤー Tank オブジェクト |

### AI の選択（Strategyパターン）

1. `EnemyManager` コンポーネントの **`Ai`** フィールドを右クリック（または横の `+` ボタン）
2. 表示されるメニューから **`ChaseAndFireAI`** を選択
3. 以下のパラメータが展開されるので設定する:

| パラメータ | 推奨値 | 説明 |
|---|---|---|
| `Detection Range` | `10` | この距離以内でプレイヤーを追跡開始 |
| `Attack Range` | `5` | この距離以内でプレイヤーを攻撃 |
| `Fire Angle Threshold` | `15` | 正面からこの角度（度）以内で射撃 |

> 後でボスAIなど別のAIに差し替えたい場合は、`Ai` フィールドを右クリック → `Change Type` で変更できます。コンポーネントへの変更は不要です。

---

# STEP 3 — Rigidbody2D の設定確認

`EnemyTank` の `Rigidbody2D` を選択し、以下の設定を確認する。

| 設定項目 | 値 |
|---|---|
| Body Type | `Dynamic` |
| Gravity Scale | `0`（トップダウン2D） |
| Freeze Rotation Z | ✅ ON（物理回転は無効、スクリプトで制御） |

---

# STEP 4 — シーンを保存

`Ctrl + S` でシーンを保存します。

---

# STEP 5 — 動作確認（プレイモード）

`▶` でプレイモードを開始し、以下をすべて確認してください。

| 確認項目 | 期待する結果 |
|---|---|
| Console にエラーが出ない | ✅ |
| Enemy がプレイヤー方向に向いて移動する | ✅ |
| Enemy がプレイヤーの射程内で弾を発射する | ✅ |
| プレイヤーの弾が Enemy に当たりダメージが入る | ✅ |
| Enemy の弾がプレイヤーに当たりダメージが入る | ✅ |
| Enemy の HP が 0 になる | ✅（死亡処理は別途実装） |

---

## トラブルシューティング

### Enemy が動かない
→ `EnemyManager` の `Player Transform` フィールドにプレイヤーがアサインされているか確認してください。  
→ `EnemyMovement` の `TankStatus` が正しくアサインされているか確認してください。

### Enemy が射撃しない
→ `EnemyBulletManager` の `Is Shoot` が ON になっているか確認してください。  
→ `EnemyBulletManager` の `Bullet` Prefab がアサインされているか確認してください。

### プレイヤーの弾が Enemy に当たらない
→ `EnemyTank` に `Collider2D` が付いているか確認してください。  
→ `EnemyBulletManager` コンポーネントが `EnemyTank` にアタッチされているか確認してください（弾の衝突判定は `BulletManagerBase` を継承したコンポーネントを検索するため）。

### ChaseAndFireAI が Inspector で選択できない
→ Unity エディタを再起動してスクリプトの再コンパイルを待ってください。

### Enemy が回転しない・回転がおかしい
→ `Rigidbody2D` の `Freeze Rotation Z` が ON になっているか確認してください（ON が正しい。物理で回転させず、スクリプトで制御します）。
