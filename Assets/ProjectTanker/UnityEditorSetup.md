# Unity エディタ作業ガイド — Shapez 風 UI 構築

> スクリプトの追加・変更は完了済み。このファイルの手順に沿って Unity エディタ側の設定を行ってください。  
> 上から順に進めると依存関係で詰まりません。

---

## 目次

1. [ThemeColor アセットの作成](#1-themecolor-アセットの作成)
2. [フォントのセットアップ](#2-フォントのセットアップ)
3. [カメラ・Canvas 背景色の設定](#3-カメラcanvas-背景色の設定)
4. [モジュール3択UI のリデザイン](#4-モジュール3択ui-のリデザイン)
5. [装備スロット UI のリデザイン](#5-装備スロット-ui-のリデザイン)
6. [インベントリ UI のリデザイン](#6-インベントリ-ui-のリデザイン)
7. [ゲーム内 HUD の作成](#7-ゲーム内-hud-の作成)
8. [動作確認チェックリスト](#8-動作確認チェックリスト)

---

## カラーリファレンス（コピー用）

```
背景         #F0EDE8
パネル       #E9E5DF
枠線         #CFC9C2
テキスト主   #3D3833
テキスト副   #7A766F
ボタン通常   #DDD8D1
ボタン強調   #B5AFA7

Earth        #7CB87E
Water        #7BAFD4
Fire         #E07A5F
Wind         #F2CC6A
None         #B0ABA3
```

> Unity の Color フィールドに16進数を入力するには、カラーピッカーを開いて左下の「#」欄にそのまま貼り付けます。

---

## 1. ThemeColor アセットの作成

`ThemeColor` は全 UI がカラーを参照する ScriptableObject です。最初に作成してください。

1. **Project ウィンドウ**で `Assets/ProjectTanker/Data/` を右クリック
2. `Create > ProjectTanker > ThemeColor` を選択
3. 生成された `ThemeColor.asset` を選択
4. Inspector で各色フィールドに[カラーリファレンス](#カラーリファレンスコピー用)の値を入力

> `ThemeColor` が見当たらない場合は Unity が再コンパイル中です。しばらく待ってから再試行してください。

---

## 2. フォントのセットアップ

> **Unity 6 の注意**: Font Asset Creator は操作タイミングによってエディタがクラッシュする既知の問題があります。  
> 以下の手順を飛ばさず丁寧に進めてください。

---

### 2-1. TMP Essentials のインポート（初回のみ）

フォント作業の前に TextMeshPro の基本アセットをインポートします。

1. Unity メニュー `Window > TextMeshPro > Import TMP Essentials` をクリック
2. ダイアログが開いたら `Import TMP Essentials` ボタンを押す
3. インポートが完了するまで待つ（`Assets/TextMesh Pro/` フォルダが生成される）

> すでにインポート済みの場合はスキップして OK です。

---

### 2-2. Noto Sans JP のダウンロードと配置

1. ブラウザで `https://fonts.google.com/` を開く
2. 検索欄に `Noto Sans JP` と入力して選択
3. 右上の **「Download family」** ボタンをクリック → ZIP がダウンロードされる
4. ZIP を解凍し、以下の2ファイルを取り出す
   - `NotoSansJP-Regular.ttf`
   - `NotoSansJP-Bold.ttf`
5. Unity の Project ウィンドウで `Assets/ProjectTanker/Art/Font/` フォルダを作成
6. 取り出した `.ttf` ファイルをそのフォルダにドラッグ&ドロップ

---

### 2-3. 日本語文字リストの準備

TextMeshPro は初期状態では英数字しか対応していません。  
日本語（ひらがな・カタカナ・漢字）を表示するには、使う文字をあらかじめ登録する必要があります。

1. ブラウザで以下の URL を開く  
   `https://gist.github.com/kgsi/ed2f1c5696a2211c1fd1e1e198c96ee4`
2. ページ内の `japanese_full.txt` の **「Raw」** ボタンをクリック
3. 表示された全テキストを `Ctrl+A` → `Ctrl+C` でコピー
4. メモ帳などに貼り付けて `japanese_full.txt` という名前で **UTF-8** で保存しておく  
   （メモ帳の場合：名前を付けて保存 → 文字コード「UTF-8」を選択）

> このファイルにはひらがな・カタカナ・JIS第1〜第2水準の漢字（約6,000字）・記号が含まれています。

---

### 2-4. Font Asset Creator の設定と生成

#### 手順

1. Unity メニュー `Window > TextMeshPro > Font Asset Creator` を開く
2. `Source Font File` に `NotoSansJP-Regular.ttf` をドラッグ&ドロップ
3. 以下の通りに設定する

| 設定項目 | 設定値 |
|---|---|
| **Sampling Point Size** | Auto Sizing |
| **Padding** | **8**（重要：0のままだとクラッシュする場合あり） |
| **Packing Method** | Fast |
| **Atlas Resolution** | **8192 × 8192**（日本語は 2048 では文字がぼやける） |
| **Character Set** | **Custom Characters** |
| **Render Mode** | SDFAA |
| **Get Kerning Pairs** | ON |

4. `Character Set` を `Custom Characters` に変更すると、テキストボックスが出現する  
5. テキストボックスを**一度クリックしてフォーカスを当て**、先ほどコピーした `japanese_full.txt` の中身を貼り付ける（`Ctrl+V`）

> ⚠️ **重要（クラッシュ対策）**  
> 貼り付けた直後、Unity 下部にプログレスバーが表示されることがあります。  
> **プログレスバーが完全に消えるまで（2〜3分）何もクリックしないで待ってください。**  
> プログレスバーが消える前に `Generate Font Atlas` を押すとクラッシュします。

6. プログレスバーが完全に消えたことを確認してから **`Generate Font Atlas`** をクリック
7. 生成が始まります（Atlas サイズと文字数によって **1〜5分** かかります）
8. 右側のプレビューに文字が並んだら完了
9. **`Save`** ボタンをクリック → 保存先として `Assets/ProjectTanker/Art/Font/` を指定して保存

#### Bold 版も同じ手順で生成

`NotoSansJP-Bold.ttf` も同じ設定で Font Asset を生成し保存します。

---

### 2-5. フォントアセットが生成できない・クラッシュする場合

**方法A: 文字数を減らして生成**

全漢字ではなく、ひらがな・カタカナ・常用漢字のみに絞る場合：

1. `Character Set` を **`Extended ASCII`** に設定
2. 追加で以下を `Custom Characters` に入力：

```
あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをんがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽぁぃぅぇぉゃゅょっーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポァィゥェォャュョッーヴ
```

3. Atlas Resolution を `4096 × 4096` に下げて生成

**方法B: Dynamic フォントを使う（おすすめ）**

事前に全文字を登録せず、実行時に必要な文字を動的に追加する方法です。  
文字登録の手間がなく、レアな漢字も自動対応できます。

1. `Source Font File` に `NotoSansJP-Regular.ttf` をセット
2. **`Character Set`** を **`ASCII`** のままにする（文字リスト不要）
3. **`Atlas Resolution`** を `1024 × 1024` に設定
4. **`Generate Font Atlas`** をクリック（数秒で完了）
5. **`Save`** で保存
6. 保存した `.asset` ファイルを Project ウィンドウで選択
7. Inspector で **`Atlas Population Mode`** を **`Dynamic`** に変更
8. **`Multi Atlas Textures`** を **ON** にする
9. `Ctrl+S` でアセットを保存

> Dynamic モードは実行時に自動的に文字テクスチャを追加するため、事前のリスト登録が不要です。  
> ゲーム規模が大きくない場合はこちらの方が簡単です。

---

### 2-6. デフォルトフォントの変更

1. `Edit > Project Settings > TextMesh Pro` を開く
2. **`Default Font Asset`** に生成した `NotoSansJP-Regular SDF.asset` をドラッグ&ドロップ

これで新規に作成する TextMeshProUGUI は自動的に日本語対応フォントを使います。

---

## 3. カメラ・Canvas 背景色の設定

### 3-1. カメラの背景色

1. Hierarchy で `MainCamera` を選択
2. Inspector の `Camera` コンポーネント → `Background` を `#F0EDE8` に変更

### 3-2. Canvas の背景パネル

Canvas 直下に全画面を覆う背景パネルがある場合：

1. Hierarchy でその `Image` GameObject を選択
2. `Image` コンポーネント → `Color` を `#F0EDE8` に変更

背景パネルがまだない場合：

1. Hierarchy で `Canvas` を右クリック → `UI > Image`
2. 名前を `Background` に変更
3. `RectTransform` の Anchor を **Stretch / Stretch**（四隅すべて）に設定
4. Left/Right/Top/Bottom をすべて `0`
5. `Color` を `#F0EDE8` に変更
6. Hierarchy の順序を一番上（最背面）に移動（`Background` を一番上にドラッグ）

---

## 4. モジュール3択UI のリデザイン

**対象シーン**: `1_MainGame.unity`  
**対象 GameObject**: `Canvas > ModuleSelectPanel_3`

### 4-1. パネル背景

1. `ModuleSelectPanel_3` の `Image` コンポーネント → `Color` を `#E9E5DF`、Alpha `230`（約90%）に変更
2. `RectTransform` のサイズを調整（例: Width 560, Height 320）
3. 画面中央に配置（Anchor を Middle/Center に）

> **角丸にするには**: 角丸の9スライス用スプライトを用意するか、Unity 2021.2+ であれば Image の `Image Type = Sliced` に角丸スプライトを使います。シンプルに始める場合はまず平面でも問題ありません。

### 4-2. 各選択肢ボタン（カード型）のリデザイン

`ModuleSelectPanel_3` 以下に3つの選択肢オブジェクト（`ModuleOptionButton`）があります。

**1つのカードの構成（他の2つも同じ構造にする）**:

```
[ModuleOptionButton] （Button コンポーネント付き）
├── AccentBar          Image（カード上部の属性カラーバー、Height: 8px）
├── IconImage          Image（アイコン表示、サイズ: 64×64）
└── NameText           TextMeshProUGUI（モジュール名）
```

#### 手順

1. 各 `ModuleOptionButton` の GameObject を選択し、Image コンポーネントのカラーを `#E9E5DF` に設定
2. 子に `UI > Image` を追加して名前を `AccentBar` に変更
   - Height: `8`、幅は親いっぱいに（Anchor: Top/Stretch）
   - Top を `0` に設定（カード上部に接する）
   - Color はスクリプトが実行時に設定するので白（#FFFFFF）のままでOK
3. アイコン用 `Image`（IconImage）のサイズを **64×64** に調整、中央に配置
4. `NameText` を `ModuleOptionButton` の下部に配置
   - フォントサイズ: 14、Color: `#3D3833`
5. `ModuleOptionButton` スクリプトの Inspector で各フィールドをアサイン：
   - `Button` → 自身の `Button` コンポーネント
   - `Icon Image` → `IconImage` オブジェクト
   - `Name Text` → `NameText` オブジェクト
   - `Accent Bar` → `AccentBar` オブジェクト
   - `Theme` → `ThemeColor.asset`

### 4-3. ボタンのホバー効果

1. 各カードの `Button` コンポーネント → `Transition` を `Color Tint` に
2. `Highlighted Color` を `#DDD8D1`、`Pressed Color` を `#B5AFA7` に変更

---

## 5. 装備スロット UI のリデザイン

**対象 GameObject**: `Canvas > Slot`

### 5-1. スロット全体の配置

1. `Slot` オブジェクトの `RectTransform` を画面下部中央に配置
   - Anchor: **Bottom/Center**
   - Pos Y: `40`（下から少し浮かせる）
2. `Slot` に `Horizontal Layout Group` コンポーネントを追加
   - Spacing: `8`
   - Child Alignment: Middle Center

### 5-2. 各 SlotItem のリデザイン

`Slot` 以下に `SlotItem` × 7 があります。1つずつ以下の構成にします（他も同じ）。

```
[SlotItem] （RectTransform: 72×80）
├── Background         Image（スロット背景）
├── IconImage          Image（モジュールアイコン、サイズ: 48×48）
├── NameText           TextMeshProUGUI（モジュール名、フォントサイズ: 10）
├── AccentLine         Image（スロット下部の属性カラーライン）
└── SlotNumber         TextMeshProUGUI（スロット番号、フォントサイズ: 10）
```

#### 手順

1. 各 `SlotItem` のルート `Image` の Color を `#E9E5DF` に変更（背景）
2. 子に `AccentLine` を追加
   - Anchor: **Bottom/Stretch**、Height: `4`、Bottom: `0`
   - Color は実行時にスクリプトが設定するので白のままでOK
3. 子に `SlotNumber` を追加
   - Anchor: **Top/Left**、サイズ: 20×16、オフセット: (2, -2)
   - フォントサイズ: 10、Color: `#7A766F`
   - Text はスクリプトが実行時に設定するので空白でOK
4. `SlotItemUI` スクリプトの Inspector で各フィールドをアサイン：
   - `Icon Image` → `IconImage`
   - `Name Text` → `NameText`
   - `Accent Line` → `AccentLine`
   - `Slot Number` → `SlotNumber`
   - `Theme` → `ThemeColor.asset`

> **7つ全部に同じ設定が必要**です。1つ設定したら Prefab 化して残りは Prefab から作ると効率的です。

---

## 6. インベントリ UI のリデザイン

**対象 GameObject**: `Canvas > ModuleInventory`  
**対象 Prefab**: `Assets/ProjectTanker/Prefab/ModuleInventoryPanel.prefab`

### 6-1. パネルの配置・スタイル

1. `ModuleInventory` の `panel` オブジェクトを選択
2. `RectTransform` を右側サイドパネルに変更：
   - Anchor: **Right/Stretch**（上下いっぱい、右端固定）
   - Width: `240`
   - Right: `0`
3. `Image` の Color: `#E9E5DF`

### 6-2. アイテム Prefab (`InventoryItemUI`) のリデザイン

`ModuleInventoryPanel.prefab` を開いてアイテムのテンプレートを編集します。

```
[InventoryItemUI] （RectTransform: 216×48）
├── IconImage        Image（サイズ: 40×40、左端）
├── NameText         TextMeshProUGUI（フォントサイズ: 13）
└── ElementDot       Image（円形、サイズ: 12×12、右端）
```

#### 手順

1. Prefab を開いてルートの `Image` Color を `#E9E5DF` に
2. 子に `ElementDot` を追加
   - Anchor: **Right/Middle**
   - サイズ: 12×12、Right: `8`
   - Sprite は `UI/Default`（円形に見せるため `Image Type` は `Simple`、`Preserve Aspect` ON）
3. `InventoryItemUI` スクリプトの Inspector で各フィールドをアサイン：
   - `Icon Image` → `IconImage`
   - `Name Text` → `NameText`
   - `Element Dot` → `ElementDot`
   - `Theme` → `ThemeColor.asset`
4. Prefab を保存（`Ctrl+S`）

### 6-3. itemContainer のレイアウト設定

1. `panel` 内の `itemContainer`（アイテムを生成する親）を選択
2. `Vertical Layout Group` を追加：
   - Spacing: `4`
   - Child Force Expand Width: ON、Height: OFF
   - Padding: Left 12, Right 12, Top 8, Bottom 8

---

## 7. ゲーム内 HUD の作成

### 7-1. HUD 用 GameObject の作成

1. Hierarchy で `Canvas` を右クリック → `Create Empty`
2. 名前を `HUD` に変更
3. `HUD` に `GameHUD` スクリプトをアタッチ

### 7-2. HP バーの作成

```
[HUD]
└── HPArea                     （Anchor: Top/Left、サイズ 200×32、Pos: (16, -16)）
    ├── HPBackground           Image（Color: #CFC9C2、角丸なら9スライス）
    ├── HPFill                 Image（Color: #E07A5F 初期値、Image Type: Filled、Fill Method: Horizontal）
    └── HPText                 TextMeshProUGUI（Color: #3D3833、フォントサイズ: 12、中央揃え）
```

#### 手順

1. `HUD` 以下に `Create Empty` → 名前 `HPArea`
   - RectTransform: Anchor Top/Left、Width: 200、Height: 32、Pos X: 16、Pos Y: -16
2. 子に `HPBackground`（Image）を追加
   - Anchor: Stretch/Stretch（全面）
   - Color: `#CFC9C2`
3. 子に `HPFill`（Image）を追加
   - Anchor: Stretch/Stretch（全面）
   - Color: `#E07A5F`
   - `Image Type`: **Filled**
   - `Fill Method`: **Horizontal**
   - `Fill Origin`: **Left**
   - `Fill Amount`: `1.0`（初期値）
4. 子に `HPText`（TextMeshProUGUI）を追加
   - Anchor: Stretch/Stretch（全面）
   - Alignment: Center/Middle
   - フォントサイズ: 12、Color: `#3D3833`

### 7-3. 弾数表示エリアの作成

```
[HUD]
└── AmmoArea                   （Anchor: Bottom/Right、Pos: (-16, 16)）
    ├── AmmoContainer          （Horizontal Layout Group、弾アイコンを横並びにする親）
    ├── AmmoBulletPrefab       Image（弾アイコン1つのテンプレート、非表示 SetActive:false）
    └── ReloadCircle           Image（Image Type: Filled、Radial 360、初期非表示）
```

#### AmmoBulletPrefab の作成

1. `AmmoArea` 以下に `UI > Image` を追加 → 名前 `AmmoBulletPrefab`
2. サイズ: **16×16**
3. Sprite に弾丸の形のアイコンをアサイン（なければ `UI/Default` でも可）
4. **GameObject を非表示に**（`SetActive(false)` で良いが、Prefab 扱いなので Inspector 上部のチェックをOFFにする）

> **注意**: `AmmoBulletPrefab` は Instantiate のテンプレートとして使うので、Hierarchy 上に置いたまま非表示にします。

#### ReloadCircle の作成

1. `AmmoArea` 以下に `UI > Image` を追加 → 名前 `ReloadCircle`
2. サイズ: **40×40**
3. Sprite に円形のスプライトをアサイン（`UI/Default` を円形にするか、専用スプライトを用意）
4. `Image Type`: **Filled**
5. `Fill Method`: **Radial 360**
6. `Fill Origin`: **Top**
7. `Fill Amount`: `0.0`（初期値）
8. Color: `#7A766F`（グレー）
9. GameObject を**非表示**に（スクリプトが制御するため）

### 7-4. GameHUD スクリプトへの参照アサイン

`HUD` オブジェクトの `GameHUD` コンポーネントの Inspector で以下をアサイン：

| フィールド | アサインするもの |
|---|---|
| `Tank Status` | `Tank` GameObject の `TankStatus` コンポーネント |
| `Bullet Manager` | `Tank` GameObject の `TankBulletManager` コンポーネント |
| `Theme` | `ThemeColor.asset` |
| `Hp Fill` | `HPFill` の `Image` コンポーネント |
| `Hp Text` | `HPText` の `TextMeshProUGUI` コンポーネント |
| `Ammo Container` | `AmmoContainer` の `Transform` |
| `Ammo Bullet Prefab` | `AmmoBulletPrefab` の `Image` コンポーネント |
| `Reload Circle` | `ReloadCircle` の `Image` コンポーネント |

---

## 8. 動作確認チェックリスト

すべての設定が完了したら、Unity でプレイモードに入って以下を確認してください。

### 基本カラー
- [ ] 画面背景がクリーム色（`#F0EDE8`）になっている
- [ ] パネル類がグレージュ（`#E9E5DF`）になっている

### モジュール3択 UI
- [ ] ゲーム開始時に3択カードが表示される
- [ ] カード上部に属性カラーバー（Earth=緑、Water=青、Fire=赤、Wind=黄）が表示される
- [ ] モジュールを選ぶとパネルが閉じてインベントリに追加される

### 装備スロット
- [ ] 画面下部に7つのスロットが表示される
- [ ] 空スロットはアイコンが非表示（alpha=0）になっている
- [ ] スロット左上に番号（1〜7）が表示される
- [ ] モジュールを装備するとスロット下部の `AccentLine` に属性色が表示される
- [ ] ドラッグ中のゴーストが半透明（70%）になっている

### インベントリ UI
- [ ] E キーでインベントリパネルが開閉する
- [ ] 各アイテムの右端に属性カラーのドットが表示される
- [ ] ドラッグ中のゴーストが半透明（70%）になっている
- [ ] スロットからインベントリへドラッグ&ドロップでモジュールが戻る

### HUD
- [ ] 左上に HP バーが表示される
- [ ] HP が減ると HP バーが縮まり、Fire（赤）→ Wind（黄）でグラデーション変化する
- [ ] 右下に弾数分のアイコンが表示される
- [ ] 弾を撃つと使用済みアイコンが薄色（`#CFC9C2`）に変わる
- [ ] 弾が0になるとリロード円形プログレスが表示され、リロード完了で消える

---

## トラブルシューティング

### スクリプトが Inspector に表示されない
→ コンパイルエラーがある可能性があります。Console ウィンドウ（`Ctrl+Shift+C`）でエラーを確認してください。

### ThemeColor が Create メニューに出ない
→ Unity がスクリプトをまだコンパイルしていません。Console にエラーがなければ少し待ってから再試行してください。

### ドラッグ&ドロップが動かない
→ Canvas の `Event System` が Hierarchy に存在するか確認してください。ない場合は `GameObject > UI > Event System` で追加します。

### フォントが文字化けする
→ TextMeshPro のフォントアセットに日本語グリフが含まれていない可能性があります。Font Asset Creator で Character Set を `Unicode Range (Hex)` にして `3000-9FFF` の範囲を追加して再生成してください。
