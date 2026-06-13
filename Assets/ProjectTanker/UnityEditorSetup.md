# Unity エディタ作業ガイド — project Tanker

# ゲームループ セットアップ

## STEP A-1 — GameManager の配置

1. Hierarchy を右クリック → `Create Empty` → 名前を **`GameManager`** に変更
2. `Add Component → GameManager`
3. Inspector で各フィールドをアサイン

| フィールド      | アサイン先                                |
| --------------- | ----------------------------------------- |
| Player Status   | PlayerTank の `TankStatus` コンポーネント |
| Result UI       | 次の手順で作る `ResultPanel`              |

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

| フィールド | アサイン先                 |
| ---------- | -------------------------- |
| Panel      | `ResultPanel` 自身         |
| Title Text | `TitleText` の TextMeshPro |

4. `ResultPanel` の `TitleText` を追加: `ResultPanel` 右クリック → `UI → Text - TextMeshPro`
5. リトライ/タイトルボタンを追加: `ResultPanel` 右クリック → `UI → Button - TextMeshPro`
   - 各ボタンの On Click に `GameResultUI.OnRetry` / `OnTitle` を設定
6. **`ResultPanel` を非活性にしておく**（Inspector の チェックボックスを OFF）
7. `GameManager` の ResultUI フィールドに `ResultPanel` をアサイン

> **クリア条件の実装方法:** 任意のスクリプトから `GameManager.TriggerClear()` を呼ぶと "STAGE CLEAR" が表示される。

---

## STEP A-3 — Build Settings に 2_title を追加

`File → Build Settings` を開き、`Assets/ProjectTanker/Scene/2_title.unity` が
リストに含まれていなければ **Add Open Scenes** またはドラッグ&ドロップで追加する。

---

# ビジュアル強化 セットアップ

## STEP B-1 — 走行時の煙エフェクト（TrackDustEffect）

タンクが走行する際、キャタピラー左右の地面から煙が立ち上がる演出を設定する。

### 子オブジェクトの作成（PlayerTank・EnemyTank 各 1 体ずつ設定）

1. タンクルートを右クリック → `Create Empty` → 名前 **`LeftTrackDust`**
   - Transform Position: `(-0.3, 0, 0)`
   - `Add Component → Particle System`
2. 同様に **`RightTrackDust`** を作成
   - Transform Position: `(0.3, 0, 0)`
   - `Add Component → Particle System`

### ParticleSystem の設定（LeftTrackDust / RightTrackDust 共通）

| 項目 | 値 |
|------|-----|
| Looping | ON |
| Start Lifetime | `0.4` |
| Start Speed | `0.8` |
| Start Size | `0.15` |
| Start Color | グレー `(0.5, 0.5, 0.5, 0.5)` ※ Alpha 128 |
| Shape → Shape | `Box` |
| Shape → Scale | `(0.1, 0.2, 0)` |
| Emission → Rate over Time | `20`（スクリプトが実行時に上書き） |
| Renderer → Order in Layer | `3`（タンク本体より手前） |
| Renderer → Material | **設定不要**（スクリプトが自動でアサイン） |

### マテリアルの作成（1 回だけ）

1. Project ウィンドウで `Assets/ProjectTanker/Art` 内を右クリック → `Create → Material` → 名前 **`TrackDust`**
2. Inspector で Shader を **`Universal Render Pipeline/Particles/Unlit`** に変更
3. **Surface Type** を `Transparent` に変更
4. **Blending Mode** を `Alpha` に変更（`Additive` にすると背景に溶けて見えなくなるので注意）

### TrackDustEffect コンポーネントのアサイン

1. タンクルートに `Add Component → TrackDustEffect`
2. Inspector でフィールドをアサイン

| フィールド    | アサイン先                              |
| ------------- | --------------------------------------- |
| Left Dust     | `LeftTrackDust` の ParticleSystem       |
| Right Dust    | `RightTrackDust` の ParticleSystem      |
| Rb            | タンクルートの Rigidbody2D              |
| Dust Material | 上で作成した `TrackDust` マテリアル     |

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

| フィールド | アサイン先                    |
| ---------- | ----------------------------- |
| Fill       | `HPBarFill` の SpriteRenderer |
| Status     | `EnemyTank` の `TankStatus`   |

---

## 動作確認（ゲームループ + ビジュアル強化）

| 確認項目                                  | 期待する結果                             |
| ----------------------------------------- | ---------------------------------------- |
| `GameManager.TriggerClear()` を呼ぶ       | "STAGE CLEAR" パネルが表示され時間停止   |
| プレイヤーの HP が 0 になる               | "GAME OVER" パネルが表示され時間停止     |
| リトライボタンを押す                      | シーンが再読込みされ再プレイできる       |
| タイトルボタンを押す                      | 2_title シーンに遷移する                 |
| タンクが前進・旋回する                    | 左右キャタピラー位置から灰色の煙が出る   |
| タンクが停止する                          | 煙が止まる                               |
| 敵にダメージを与える                      | 頭上の赤バーが縮む                       |
