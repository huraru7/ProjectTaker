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

---

# 通知システム セットアップ

## STEP C-1 — NotificationManager の配置

### Canvas 階層の構築

既存の Canvas 直下に以下の階層を作成する。

```
Canvas
└── NotificationRoot  ← 空 GameObject（整理用）
    └── NotificationContainer  ← RectTransform、NotificationManager.cs をアタッチ
```

1. Canvas を右クリック → `Create Empty` → 名前 **`NotificationRoot`**
2. `NotificationRoot` の子に `Create Empty` → 名前 **`NotificationContainer`**
3. `NotificationContainer` の RectTransform を以下に設定

| プロパティ | 値 |
|---|---|
| Anchor Min / Max | `(0, 1)` / `(0, 1)` |
| Pivot | `(0, 1)` |
| Pos X | `16` |
| Pos Y | `-16` |
| Width | `340` |

4. `NotificationContainer` に `Add Component → NotificationManager`
5. Inspector でフィールドをセット（Entry Prefab は次の手順で作成）

| フィールド | 設定内容 |
|---|---|
| Entry Prefab | 次手順で作る `NotificationEntry` Prefab |
| Default Duration | `3.5` |
| Max Entries | `5` |
| Entry Spacing | `8` |
| Top Margin | `16` |
| Container | `NotificationContainer` の RectTransform 自身 |

---

## STEP C-2 — NotificationEntry Prefab の作成

### Prefab 階層

```
NotificationEntry  (RectTransform 320×64、CanvasGroup)
├── Background    (Image、#1E2228、alpha 0.92)
├── AccentBar     (Image、4px 幅、縦ストレッチ)
├── IconRoot      (空 GameObject)
│   └── Icon     (Image、36×36)
└── MessageText   (TextMeshProUGUI、size 13、白、左中央揃え)
```

**作成手順**

1. `NotificationContainer` 下に `Create Empty` → 名前 **`NotificationEntry`**
2. RectTransform サイズを `(320, 64)` に設定
3. `Add Component → CanvasGroup`
4. `Add Component → NotificationEntry`
5. 子 **`Background`** を作成
   - `UI → Image`
   - Color: `#1E2228`、Alpha: `0.92`
   - Stretch 設定（Anchor を四隅に広げる）
6. 子 **`AccentBar`** を作成
   - `UI → Image`
   - Anchor: 左端に縦ストレッチ（Min `(0,0)` / Max `(0,1)`）
   - Width: `4`、Pos X: `0`
   - Color はスクリプトが動的に変更するため任意
7. 子 **`IconRoot`** を作成（`Create Empty`）
   - その子に `UI → Image` → 名前 **`Icon`**、サイズ `(36, 36)`
8. 子 **`MessageText`** を作成
   - `UI → Text - TextMeshPro`
   - Font Size: `13`、Color: `白`、Alignment: 左中央
   - 左マージンを AccentBar + Icon 分だけ確保（Padding Left: 50 程度）
9. `NotificationEntry` コンポーネントの Inspector でフィールドをアサイン

| フィールド | アサイン先 |
|---|---|
| Accent Bar | `AccentBar` の Image |
| Icon Image | `Icon` の Image |
| Icon Root | `IconRoot` GameObject |
| Message Text | `MessageText` の TextMeshProUGUI |

10. `NotificationEntry` を Prefab 化（`Assets/ProjectTanker/Prefab/` にドラッグ）
11. シーン上の仮オブジェクトは削除し、Prefab を `NotificationManager` の `Entry Prefab` フィールドにセット

---

## STEP C-3 — TutorialManager の配置（オプション）

チュートリアルステップを Inspector から設定して順番に通知を出したい場合に追加する。

1. Hierarchy で `Create Empty` → 名前 **`TutorialManager`**
2. `Add Component → TutorialManager`
3. Inspector の `Steps` リストに各ステップを追加

| フィールド | 設定内容 |
|---|---|
| Message | 表示するテキスト（TextArea） |
| Duration | 表示時間（秒）。デフォルト `5` |
| Delay Before | このステップの前に待つ時間（秒） |
| Icon | アイコン Sprite（任意） |

**コードからチュートリアルを開始する場合**

```csharp
TutorialManager.Instance.StartTutorial("intro");
```

`StartTutorial` は同じ ID が完了済みであれば自動でスキップする（`force: true` で強制再実行）。

---

## STEP C-4 — 通知を呼び出す

```csharp
// 基本（青）
NotificationManager.Show("メッセージ");

// チュートリアル（黄）
NotificationManager.Tutorial("弾を撃ってスイッチを ON にしよう！");

// 警告（オレンジ）
NotificationManager.Warning("弾数が残り少ない");

// 成功（緑）
NotificationManager.Success("ドアを開けた！");
```

---

## 動作確認（通知システム）

| 確認項目 | 期待する結果 |
|---|---|
| `NotificationManager.Show("テスト")` を呼ぶ | 左上からスライドインして表示される |
| 複数回連続で呼ぶ | 古い通知が下へ押し出されて並ぶ |
| 6 件以上呼ぶ | 最古の通知が消えて 5 件以内に収まる |
| 一定時間後 | フェードアウトして自動消滅する |

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
