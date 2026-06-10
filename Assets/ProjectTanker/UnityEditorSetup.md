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

---

---

# ビジュアル実装セットアップ — エフェクト・シェーダー・HUD

> コード側の変更（スクリプト実装）が完了してから、このガイドに沿って Unity エディタ側を整理してください。
> **STEP 1〜5（Enemy System）が完了している前提**です。

---

## カラーリファレンス（ビジュアル系）

```
ベース背景        #F5F6F8
グリッド          #C8D8E4
プレイヤータンク  #4DB8E8
プレイヤー影      #2E8FB5
敵タンク          #FF6B6B
敵タンク影        #CC4444
弾・UIアクセント  #F5C518
壁               #5D6470
壁エッジ          #70767F
爆発オレンジ      #FF9F43
UI背景（暗）     #1E2228
アウトライン      #1A1A1A
```

---

# STEP 6 — マテリアルの作成

## 6-1. マテリアル保存フォルダを作成する

1. Project ウィンドウで `Assets/ProjectTanker/Art/` を右クリック
2. `Create → Folder` → 名前を **`Materials`** に変更

---

## 6-2. 各マテリアルを作成する

以下の手順を **7 回**繰り返してマテリアルを作成します。

1. `Assets/ProjectTanker/Art/Materials/` を右クリック
2. `Create → Material` → 下表の名前を入力
3. Inspector の **Shader** プルダウンを `Custom/（シェーダー名）` に変更
4. 下表の設定値を入力する

| マテリアル名 | シェーダー | 設定内容 |
|---|---|---|
| `DotGround` | `Custom/DotGround` | Background Color: `#F5F6F8`、Dot Color: `#C8D8E4`、Dot Spacing: `1.0`、Dot Radius: `0.07`、Pulse Speed: `0.8`、Pulse Amount: `0.06` |
| `PlayerTank` | `Custom/CelShading` | Base Color: `#4DB8E8`、Shadow Color: `#2E8FB5`、Steps: `2` |
| `EnemyTank` | `Custom/CelShading` | Base Color: `#FF6B6B`、Shadow Color: `#CC4444`、Steps: `2` |
| `TankOutline` | `Custom/Outline` | Outline Color: `#1A1A1A`（太さは GameObject の Scale で制御：1.05 推奨） |
| `HPBar` | `Custom/HPBar` | 全項目デフォルト値のまま（変更不要） |
| `ArmorBar` | `Custom/HPBar` | Color Full: `#4DB8E8`、Color Mid: `#4DB8E8`、Color Low: `#8888FF` |
| `WallTile` | `Custom/WallTile` | Wall Color: `#5D6470`、Edge Highlight: `#70767F`、Edge Width: `0.06` |

---

# STEP 7 — マテリアルを各オブジェクトに適用する

## 7-1. 背景グラウンドを作成する

1. Hierarchy を右クリック → `3D Object → Quad` → 名前を **`Ground`** に変更
2. Inspector の Transform を以下に設定する

| 項目 | 値 |
|---|---|
| Position | `(0, 0, 5)`（Z=5 で他のオブジェクトより奥に表示） |
| Rotation | `(0, 0, 0)` |
| Scale | `(100, 100, 1)` |

3. `MeshRenderer` コンポーネントの **Materials の Element 0** に `DotGround` マテリアルをアサイン

---

## 7-2. プレイヤータンクにマテリアルを適用する

Tank の Hierarchy はこのような 2 階層構造になっています。

```
tank_boby          ← 空の親オブジェクト（仕分け用コンテナ）
└── tank_boby      ← スプライトを持つ子オブジェクト（SpriteRenderer あり）
    ├── caterpillar
    └── ...
```

**マテリアルを適用するのは「SpriteRenderer コンポーネントが付いている方の `tank_boby`（内側・子）」です。**  
外側の親 `tank_boby` はコンテナなので何も変更しません。

### 子 `tank_boby`（胴体スプライト）へのマテリアル適用手順

1. Hierarchy で `tank_boby` を展開する
2. 子の `tank_boby` を選択し、Inspector で **`SpriteRenderer`** コンポーネントが表示されることを確認する
3. `SpriteRenderer` の **Material** フィールドに `PlayerTank` をアサイン

> 見分け方: Inspector に `SpriteRenderer` コンポーネントがあり、**Sprite** フィールドにスプライト画像が設定されている方が正しい対象です。

### アウトラインの追加
1. `tank_boby` を選択 → `Add Component → Mesh Renderer`（または SpriteRenderer を使用している場合は2番目のマテリアルスロット）
2. SpriteRenderer は複数マテリアルを直接サポートしていないため、**アウトラインは別の方法**で適用する（下記参照）

> **アウトライン適用の推奨方法**: 子 `tank_boby` の子として空 GameObject `Outline` を作成し、同じスプライトを持つ `SpriteRenderer` を追加して `TankOutline` マテリアルをアサインし、Scale を `(1.05, 1.05, 1)` に設定する。Order in Layer をメインの `tank_boby` より小さい値（例: -1）にすることでアウトラインが背後に描画される。

---

## 7-3. エネミータンクにマテリアルを適用する

STEP 7-2 と同様に、`EnemyTank` の各部位に以下を適用します。

| オブジェクト | マテリアル |
|---|---|
| `tank_boby` の SpriteRenderer | `EnemyTank` |
| アウトライン用 GameObject の SpriteRenderer | `TankOutline` |

---

## 7-4. 壁にマテリアルを適用する

シーン内の Wall オブジェクト（またはそのプレハブ）を開き、`SpriteRenderer` の **Material** に `WallTile` を適用します。

> Tilemap を使用している場合は `TilemapRenderer` の Material フィールドに `WallTile` をアサインしてください。

---

# STEP 8 — BarrelController のセットアップ

> バレル独立回転システム（Stage 1）のスクリプトが実装済みの前提です。

## 8-1. プレイヤータンクへのバレルセットアップ

1. Tank Prefab（またはシーン内の Tank オブジェクト）を開く
2. 子オブジェクト **`tank_barrel`** を選択
3. `Add Component → BarrelController` を追加
4. `_muzzleOffset` を `1.0` に設定（デフォルト値）

5. **Tank のルートオブジェクト**を選択し、各コンポーネントの `_barrel` フィールドに `tank_barrel` をアサイン

| コンポーネント | フィールド | アサインするもの |
|---|---|---|
| `TankMovement` | `Barrel` | `tank_barrel` オブジェクトの `BarrelController` |
| `TankBulletManager` | `Barrel` | `tank_barrel` オブジェクトの `BarrelController` |

6. `TankStatus` の `TankData` に `barrelTurnRate` が追加されているので、値を確認する（推奨: **120**）

---

## 8-2. エネミータンクへのバレルセットアップ

1. EnemyTank Prefab（またはシーン内の EnemyTank オブジェクト）を開く
2. 子オブジェクト **`tank_barrel`** を選択
3. `Add Component → BarrelController` を追加
4. `_muzzleOffset` を `1.0` に設定

5. **EnemyTank のルートオブジェクト**を選択し、以下をアサイン

| コンポーネント | フィールド | アサインするもの |
|---|---|---|
| `EnemyManager` | `Barrel` | `tank_barrel` の `BarrelController` |
| `EnemyBulletManager` | `Barrel` | `tank_barrel` の `BarrelController` |

---

# STEP 9 — エフェクト関連のセットアップ

## 9-1. BulletMark Prefab を作成する

1. Hierarchy を右クリック → `Create Empty` → 名前を **`BulletMark`** に変更
2. `Add Component → SpriteRenderer` を追加し、以下を設定する

| 設定項目 | 値 |
|---|---|
| Sprite | Unity 内蔵の `Sprites/Circle`（プロジェクトウィンドウの検索で "Circle" と入力して見つける） |
| Color | `#1A1A1A`、Alpha: **100**（25% 程度） |
| Order in Layer | `1` |

3. Transform の Scale を `(0.25, 0.25, 1)` に設定
4. `Add Component → BulletMark` を追加
5. **Project ウィンドウにドラッグ**して Prefab 化（保存先: `Assets/ProjectTanker/Art/Prefabs/` などを作成して保存）
6. Hierarchy から削除

---

## 9-2. Explosion Prefab を作成する

### ルートオブジェクト作成

1. Hierarchy を右クリック → `Create Empty` → 名前を **`Explosion`** に変更
2. `Add Component → ExplosionEffect` を追加

### Flash 子オブジェクト

1. `Explosion` を右クリック → `Effects → Particle System` → 名前を **`Flash`** に変更
2. Particle System を以下に設定する

| 項目 | 値 |
|---|---|
| Start Color | `白（White）` |
| Start Size | `1.5` |
| Start Lifetime | `0.08` |
| Start Speed | `0` |
| Duration | `0.1` |
| Looping | `OFF` |
| Emission → Bursts | `0秒、Count: 1` |
| Renderer → Render Mode | `Billboard` |

### Fireball 子オブジェクト

1. `Explosion` を右クリック → `Effects → Particle System` → 名前を **`Fireball`** に変更

| 項目 | 値 |
|---|---|
| Start Color | グラデーション `#FF9F43` → `#FF6B6B` |
| Start Size | ランダム `0.3` ～ `0.8` |
| Start Lifetime | ランダム `0.4` ～ `0.6` |
| Start Speed | ランダム `2.0` ～ `5.0` |
| Duration | `0.5` |
| Looping | `OFF` |
| Emission → Bursts | `0秒、Count: 12` |
| Shape → Shape | `Sphere`、Radius: `0.2` |
| Size over Lifetime | `1.0 → 0.0`（縮小カーブ） |

### Debris 子オブジェクト

1. `Explosion` を右クリック → `Effects → Particle System` → 名前を **`Debris`** に変更

| 項目 | 値 |
|---|---|
| Start Color | `#5D6470`（グレー） |
| Start Size | ランダム `0.1` ～ `0.2` |
| Start Lifetime | ランダム `0.6` ～ `1.0` |
| Start Speed | ランダム `3.0` ～ `8.0` |
| Gravity Source | `None`（2D なので重力なし） |
| Duration | `1.0` |
| Looping | `OFF` |
| Emission → Bursts | `0秒、Count: 8` |
| Shape → Shape | `Sphere`、Radius: `0.1` |

### 参照のアサイン

`Explosion` ルートの `ExplosionEffect` コンポーネントに以下をアサイン

| フィールド | アサインするもの |
|---|---|
| `Flash` | 子の `Flash` ParticleSystem |
| `Fireball` | 子の `Fireball` ParticleSystem |
| `Debris` | 子の `Debris` ParticleSystem |

### Prefab 化

- Project ウィンドウにドラッグして Prefab として保存
- Hierarchy から削除

---

## 9-3. EffectManager オブジェクトを Scene に作成する

1. Hierarchy を右クリック → `Create Empty` → 名前を **`EffectManager`** に変更
2. 以下のコンポーネントを `Add Component` で追加し、それぞれ設定する

### ScreenFlash

| 手順 | 操作 |
|---|---|
| 1 | `Add Component → ScreenFlash` |
| 2 | `_flashImage` フィールドには後で STEP 10 で作成する全画面白 Image を設定する |

### HitStop

| 手順 | 操作 |
|---|---|
| 1 | `Add Component → HitStop` |
| 2 | 設定項目なし |

### CameraShake

| 手順 | 操作 |
|---|---|
| 1 | `Add Component → CameraShake` |
| 2 | `_cameraTransform` に シーンの `Main Camera` をアサイン（空欄なら自動で `Camera.main` を使用） |

### BulletMarkSpawner

| 手順 | 操作 |
|---|---|
| 1 | `Add Component → BulletMarkSpawner` |
| 2 | `_markPrefab` に STEP 9-1 で作成した `BulletMark` Prefab をアサイン |

### ExplosionEffectSpawner

| 手順 | 操作 |
|---|---|
| 1 | `Add Component → ExplosionEffectSpawner` |
| 2 | `_prefab` に STEP 9-2 で作成した `Explosion` Prefab をアサイン |

---

## 9-4. Bullet Prefab に LineRenderer を追加する

> スクリプト（`Bullet.cs`）が `[RequireComponent(typeof(LineRenderer))]` を持つため、Add Component すれば自動でアタッチされます。スクリプト側で色・幅は自動設定されます。

1. `Bullet.prefab` を Project ウィンドウでダブルクリックして Prefab 編集モードに入る
2. `Add Component → Line Renderer` を追加（または既にある場合はそのまま）
3. Line Renderer の **Material** を `Sprites/Default`（Unity 内蔵マテリアル）に変更する
4. Prefab を保存して閉じる

---

# STEP 10 — HUD / Canvas のセットアップ

## 10-1. Canvas の確認・作成

既存の Canvas があればそれを使用します。なければ以下の手順で作成します。

1. Hierarchy を右クリック → `UI → Canvas`
2. Canvas コンポーネントを以下に設定

| 項目 | 値 |
|---|---|
| Render Mode | `Screen Space - Overlay` |
| Canvas Scaler → UI Scale Mode | `Scale With Screen Size` |
| Canvas Scaler → Reference Resolution | `1920 × 1080` |
| Canvas Scaler → Match | `0.5`（Width と Height の中間） |

---

## 10-2. ScreenFlash Image を追加する

1. Canvas を右クリック → `UI → Image` → 名前を **`ScreenFlash`** に変更
2. Rect Transform を**全画面ストレッチ**に設定する
   - Anchor Presets で `Stretch Stretch`（四隅）を選択
   - Left/Right/Top/Bottom をすべて `0` に設定
3. Image の Color を `白（White）`、Alpha を `0` に設定
4. **EffectManager** の `ScreenFlash` コンポーネントの `_flashImage` に、この Image をアサイン

---

## 10-3. HP バーのルート RectTransform をアサインする

既存の `GameHUD` コンポーネントに `_hpBarRoot` フィールドが追加されています。

1. `GameHUD` がアタッチされている GameObject を選択
2. `_hpBarRoot` フィールドに、**HP Slider（または HP バー全体を囲む親 RectTransform）** をアサイン

> このフィールドに設定した RectTransform がダメージ時に横に揺れます。HP Fill Image より 1〜2 階層上の親コンテナを指定すると自然な見た目になります。

---

# STEP 11 — ビジュアル動作確認

プレイモード（`▶`）で以下をすべて確認してください。

| 確認項目 | 期待する結果 |
|---|---|
| 背景に薄いドットのグリッドが表示されている | ✅ DotGround.mat が適用されている |
| ドットが微細に脈動している | ✅ Pulse が動作している |
| プレイヤータンクが水色のトゥーンシェーディング | ✅ PlayerTank.mat 適用済み |
| エネミータンクが赤のトゥーンシェーディング | ✅ EnemyTank.mat 適用済み |
| 両タンクに黒いアウトラインがある | ✅ TankOutline.mat 適用済み |
| 壁がグレーでエッジハイライト付き | ✅ WallTile.mat 適用済み |
| 弾を発射すると黄色い残光（トレイル）が付く | ✅ LineRenderer トレイル動作中 |
| 弾が壁に当たると灰色の小さな丸跡が残る | ✅ BulletMark スポーン中 |
| 壁跡が約10秒かけてフェードアウトする | ✅ LitMotion フェード動作中 |
| ダメージを受けると HP バーが横に揺れる | ✅ LitMotion Shake 動作中 |
| 上下矢印キーでバレルが独立して回転する | ✅ BarrelController 動作中 |
| バレルの向いた方向に弾が飛ぶ | ✅ MuzzlePosition 発射 |

---

## トラブルシューティング（ビジュアル系）

### CelShading マテリアルを当てると真っ黒になる
→ これは古い `CelShading.shader`（法線ベースの 3D ライティング）が 2D スプライトメッシュに対応していないことが原因でした。スプライトメッシュには 3D 法線データがないため NaN が発生し黒になります。  
→ 現在のシェーダーはすでに **テクスチャ輝度ベースのセルシェーディング**に書き直されており、2D スプライトで正しく動作します。  
→ それでも黒い場合は、マテリアルの `Shadow Bias` を `0.4` → `0.0` まで下げて輝度しきい値を調整してください。

### 背景が真っ白になる / DotGround が表示されない
→ Quad の Z 位置が `5` になっているか確認。Camera の Far Clip Plane が 5 以上あるか確認。

### LineRenderer トレイルが表示されない
→ Bullet Prefab に `LineRenderer` コンポーネントが追加されているか確認。Material が `Sprites/Default` になっているか確認。

### 被弾跡が表示されない
→ EffectManager の `BulletMarkSpawner._markPrefab` に BulletMark Prefab がアサインされているか確認。

### ScreenFlash が動作しない
→ EffectManager の `ScreenFlash._flashImage` に Canvas 内の ScreenFlash Image がアサインされているか確認。

### HP バーが揺れない
→ `GameHUD._hpBarRoot` に RectTransform がアサインされているか確認。
