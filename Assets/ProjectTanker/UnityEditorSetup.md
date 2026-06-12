# Unity エディタ作業ガイド — project Tanker

---

# Tilemap マップ作成セットアップ

## STEP 1 — レイヤーの追加

1. `Edit → Project Settings → Tags and Layers` を開く
2. **User Layer** の空欄に **`Wall`** を追加する

---

## STEP 1-b — 三角タイルの自動生成（初回のみ）

1. Unity メニューバーの **`Tools → ProjectTanker → Generate Tile Sprites`** を実行する
2. `Assets/ProjectTanker/Art/Tiles/` に以下が自動生成される

| ファイル | 形状 |
|---|---|
| `Triangle_BL.png` / `Triangle_BL_Tile.asset` | 直角が左下の三角 |
| `Triangle_BR.png` / `Triangle_BR_Tile.asset` | 直角が右下の三角 |
| `Triangle_TL.png` / `Triangle_TL_Tile.asset` | 直角が左上の三角 |
| `Triangle_TR.png` / `Triangle_TR_Tile.asset` | 直角が右上の三角 |

3. `WallTile.mat` が各 Tile アセットに適用されていない場合は、Tile Palette にドラッグ後、
   `TilemapRenderer` の Material が `WallTile.mat` になっていれば自動で全タイルに適用される（Tilemap 単位の設定のため個別設定不要）

---

## STEP 2 — Tile Asset の作成

1. Project ウィンドウの `Assets/ProjectTanker/Art/` を右クリック → `Create → Folder` → **`Tiles`** を作成
2. `Assets/ProjectTanker/Art/Tiles/` を右クリック → `Create → 2D → Tiles → Tile` → 名前を **`WallTile`** に変更
3. 作成された `WallTile` を選択し Inspector で以下を設定する

| フィールド | 値 |
|---|---|
| **Sprite** | Unity 組み込みの `Sprites/Square`（Project 検索で "Square" と入力して見つける）または白い正方形スプライト |
| **Collider Type** | `Sprite` |

---

## STEP 3 — Tilemap の作成

1. Hierarchy を右クリック → `2D Object → Tilemap → Rectangular`
2. `Grid` と その子 `Tilemap` が自動生成される
3. `Tilemap` の名前を **`WallMap`** に変更する
4. `Grid` コンポーネントの **Cell Size** を `(1, 1, 0)` に確認・設定する

---

## STEP 4 — WallMap のコンポーネント設定

`WallMap` を選択し以下の順番でコンポーネントを追加・設定する。

### TilemapRenderer（自動追加済み）

| 項目 | 値 |
|---|---|
| Material | `WallTile.mat`（`Assets/ProjectTanker/Art/Materials/WallTile`）をアサイン |
| Order in Layer | タンクの SpriteRenderer より小さい値（例: `0`、タンクが `1` の場合） |

### TilemapCollider2D

1. `Add Component → TilemapCollider2D` を追加
2. **`Used By Composite`** を **ON** にする

### CompositeCollider2D

1. `Add Component → CompositeCollider2D` を追加
2. **Geometry Type** を `Polygons` に設定
3. 自動追加される `Rigidbody2D` の **Body Type** を **`Static`** に変更する

### Wall スクリプト

1. `Add Component → Wall` を追加する

> これだけで弾の反射判定（Bullet.cs）と AI の壁回避（AIUtil.cs）が自動的に機能する。

### Layer 設定

`WallMap` の Inspector 上部 **Layer** プルダウンを **`Wall`** に変更する。
「子オブジェクトも変更しますか？」と聞かれた場合は **Yes** を選択。

---

## STEP 5 — Tile Palette でマップを描く

1. `Window → 2D → Tile Palette` を開く
2. **`Create New Palette`** をクリック → 名前 **`TankMapPalette`** → 保存先に `Assets/ProjectTanker/Art/Tiles/` を指定
3. `Assets/ProjectTanker/Art/Tiles/WallTile` をパレットウィンドウにドラッグ&ドロップ
4. Tile Palette 上部で **`Active Tilemap`** が `WallMap` になっていることを確認
5. **ペイントツール（鉛筆アイコン）** を選択し Scene ビューでクリック/ドラッグして壁を描く
6. 間違えた場合は **消しゴムツール** で削除する

---

## STEP 6 — 旧 Wall オブジェクトの削除

Hierarchy から以下を削除する（Tilemap に置き換え済みのため不要）

- `Square`
- `Square (1)`
- その他 `Wall` コンポーネントが付いた個別 GameObject

---

## 動作確認

プレイモード（`▶`）で以下を確認する。

| 確認項目 | 期待する結果 |
|---|---|
| 壁が `#5D6470` のグレーで表示される | WallTile.mat の Wall Color が適用されている |
| タンクが壁を通り抜けない | TilemapCollider2D + CompositeCollider2D が機能している |
| 弾が壁で反射する | Bullet.cs の Wall 判定が Tilemap を検出している |
| AI が壁を回避する | AIUtil の Raycast が Tilemap に当たっている |
| Console にエラーなし | Wall コンポーネントが正しく機能している |

---

## トラブルシューティング

### 壁が表示されない / 真っ黒になる
→ `TilemapRenderer` の Material に `WallTile.mat` がアサインされているか確認。

### 弾が壁を無視して通り抜ける
→ `WallMap` に `Wall` コンポーネントがアタッチされているか確認。
→ `TilemapCollider2D` の `Used By Composite` が ON になっているか確認。

### AI が壁を認識しない
→ `WallMap` の Layer が `Wall` に設定されているか確認。
→ `TilemapCollider2D` が有効になっているか確認。

### タイルを描いても何も表示されない
→ Tile Palette の `Active Tilemap` が `WallMap` になっているか確認。
→ Scene ビューと Game ビューで Camera の Culling Mask に該当 Layer が含まれているか確認。

---

# ゲームループ セットアップ

## STEP A-1 — GameManager の配置

1. Hierarchy を右クリック → `Create Empty` → 名前を **`GameManager`** に変更
2. `Add Component → GameManager`
3. Inspector で各フィールドをアサイン

| フィールド | アサイン先 |
|---|---|
| Player Status | PlayerTank の `TankStatus` コンポーネント |
| Enemy Container | Hierarchy の `EnemyContainer` オブジェクト |
| Result UI | 次の手順で作る `ResultPanel` |

---

## STEP A-2 — Result UI の作成

既存の Canvas 直下に以下の GameObject 階層を作成する。

```
Canvas
└── ResultPanel  ← Panel コンポーネント、GameResultUI.cs をアタッチ
    ├── TitleText  ← TextMeshPro "STAGE CLEAR"
    ├── RetryButton  ← Button → On Click: GameResultUI.OnRetry
    └── TitleButton  ← Button → On Click: GameResultUI.OnTitle
```

1. `Canvas` を右クリック → `UI → Panel` → 名前を **`ResultPanel`** に変更
2. `ResultPanel` に `Add Component → GameResultUI`
3. Inspector でフィールドをアサイン

| フィールド | アサイン先 |
|---|---|
| Panel | `ResultPanel` 自身 |
| Title Text | `TitleText` の TextMeshPro |

4. `ResultPanel` の `TitleText` を追加: `ResultPanel` 右クリック → `UI → Text - TextMeshPro`
5. リトライ/タイトルボタンを追加: `ResultPanel` 右クリック → `UI → Button - TextMeshPro`
   - 各ボタンの On Click に `GameResultUI.OnRetry` / `OnTitle` を設定
6. **`ResultPanel` を非活性にしておく**（Inspector の チェックボックスを OFF）
7. `GameManager` の ResultUI フィールドに `ResultPanel` をアサイン

---

## STEP A-3 — Build Settings に 2_title を追加

`File → Build Settings` を開き、`Assets/ProjectTanker/Scene/2_title.unity` が
リストに含まれていなければ **Add Open Scenes** またはドラッグ&ドロップで追加する。

---

# ビジュアル強化 セットアップ

## STEP B-1 — キャタピラースクロール

### tank_tracks.png の Import 設定変更

1. Project ウィンドウで `Assets/ProjectTanker/Art/Image/tank_tracks.png` を選択
2. Inspector → **Wrap Mode** を `Repeat` に変更
3. `Apply` を押す

### プレイヤータンクへの設定

1. `PlayerTank` の子にある tracks SpriteRenderer を持つオブジェクトを選択
2. `Add Component → TrackScroller`
3. Inspector でフィールドをアサイン

| フィールド | アサイン先 |
|---|---|
| Track Renderer | tracks オブジェクトの SpriteRenderer |
| Rb | PlayerTank ルートの Rigidbody2D |

### エネミータンクへの設定

1. `EnemyTank` の子の tracks SpriteRenderer を持つオブジェクトを選択
2. 上記と同様に `TrackScroller` をアタッチし、エネミータンクの Rigidbody2D をアサイン

---

## STEP B-2 — エネミー HP バー

`EnemyTank` の子に以下を作成する（複数エネミーがいる場合は 1 体ずつ設定）。

1. `EnemyTank` を右クリック → `Create Empty` → 名前 **`HPBar`**
   - Transform: Position `(0, 0.8, 0)`（頭上）
2. `HPBar` の子に `Create Empty` → 名前 **`HPBarBG`**
   - `Add Component → SpriteRenderer`
   - Sprite: Unity 組み込みの白い正方形（`Sprites/Square` を検索）
   - Color: 白 `(0.3, 0.3, 0.3)`（濃いグレー=背景）
   - Scale: `(0.6, 0.08, 1)`
   - Order in Layer: `5`
3. `HPBar` の子に `Create Empty` → 名前 **`HPBarFill`**
   - `Add Component → SpriteRenderer`
   - Sprite: 白い正方形
   - Color: 赤 `(1, 0.2, 0.2)`
   - Scale: `(0.6, 0.08, 1)`（HPBarBG と同じ）
   - Order in Layer: `6`
4. `HPBar` に `Add Component → EnemyHPBar`
5. Inspector でフィールドをアサイン

| フィールド | アサイン先 |
|---|---|
| Fill | `HPBarFill` の SpriteRenderer |
| Status | `EnemyTank` の `TankStatus` |

---

## 動作確認（ゲームループ + ビジュアル強化）

| 確認項目 | 期待する結果 |
|---|---|
| 敵タンクの HP を 0 にする | 爆発エフェクト再生 → EnemyTank が非表示 |
| 全敵が倒される | "STAGE CLEAR" パネルが表示され時間停止 |
| プレイヤーの HP が 0 になる | "GAME OVER" パネルが表示され時間停止 |
| リトライボタンを押す | シーンが再読込みされ再プレイできる |
| タンクが前進する | キャタピラーのテクスチャが縦方向に流れる |
| 敵にダメージを与える | 頭上の赤バーが縮む |
