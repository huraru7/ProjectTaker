# Unity エディタ作業ガイド — project Tanker

---

# Tilemap マップ作成セットアップ

## STEP 1 — レイヤーの追加

1. `Edit → Project Settings → Tags and Layers` を開く
2. **User Layer** の空欄に **`Wall`** を追加する

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
