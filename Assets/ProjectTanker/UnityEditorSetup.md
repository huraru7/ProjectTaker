# Unity エディタ作業ガイド — Shapez 風 UI 構築

> スクリプトの追加・変更は完了済み。このファイルの手順に沿って Unity エディタ側の設定を行ってください。

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

> Unity の Color フィールドに16進数を入力するには、カラーピッカーを開いて左下の「#」欄に貼り付けます。

---

## ✅ 完了済み作業（1〜6）

| # | 作業内容 | 備考 |
|---|---|---|
| 1 | ThemeColor アセットの作成 | `Assets/ProjectTanker/Data/ThemeColor.asset` |
| 2 | フォントのセットアップ | NotoSansJP を Dynamic モードで生成、デフォルト設定済み |
| 3 | カメラ・Canvas 背景色の設定 | カメラ背景・背景パネルを `#F0EDE8` に変更済み |
| 4 | モジュール3択UI のリデザイン | AccentBar・カード型レイアウト・ホバー効果を設定済み |
| 5 | 装備スロット UI のリデザイン | AccentLine・SlotNumber・ThemeColor アサイン済み |
| 6 | インベントリ UI のリデザイン | ElementDot・Vertical Layout Group・Prefab 保存済み |

---

## 目次（残り作業）

7. [ゲーム内 HUD の作成](#7-ゲーム内-hud-の作成)
8. [動作確認・テスト手順](#8-動作確認テスト手順)
9. [トラブルシューティング](#9-トラブルシューティング)

---

## 7. ゲーム内 HUD の作成

HUD はゲームプレイ中に常時表示される HP バーと弾数表示です。  
`GameHUD.cs` スクリプトは作成済みなので、Unity 側で GameObject とコンポーネントを組み立てます。

### 7-1. HUD の親 GameObject を作成

1. Hierarchy で `Canvas` を右クリック → **`Create Empty`**
2. 名前を `HUD` に変更
3. `RectTransform` の Anchor を **Stretch / Stretch**（四隅すべて）に設定し Left/Right/Top/Bottom をすべて `0` にする（Canvas 全体を覆う）
4. `HUD` を選択した状態で Inspector の **`Add Component`** → `GameHUD` を検索してアタッチ

---

### 7-2. HP バーの作成

HP バーは画面左上に固定します。

#### GameObject の構成

```
HUD
└── HPArea
    ├── HPBackground   （背景の枠）
    ├── HPFill         （実際に縮むバー）
    └── HPText         （"12 / 12" のテキスト）
```

#### 手順

**HPArea を作成する**

1. `HUD` を右クリック → `Create Empty` → 名前を `HPArea` に変更
2. `RectTransform` を以下に設定：

| 項目 | 値 |
|---|---|
| Anchor | **Top Left**（左上固定） |
| Pivot | `0, 1` |
| Pos X | `16` |
| Pos Y | `-16` |
| Width | `200` |
| Height | `32` |

**HPBackground を作成する**

1. `HPArea` を右クリック → `UI > Image` → 名前を `HPBackground` に変更
2. Anchor を **Stretch / Stretch**（全面）に設定、Left/Right/Top/Bottom = `0`
3. `Color` を `#CFC9C2` に設定

**HPFill を作成する**

1. `HPArea` を右クリック → `UI > Image` → 名前を `HPFill` に変更
2. Anchor を **Stretch / Stretch**（全面）に設定、Left/Right/Top/Bottom = `0`
3. `Color` を `#E07A5F` に設定（スクリプトが HP 残量に応じて変化させる）
4. `Image Type` を **`Filled`** に変更
5. `Fill Method` を **`Horizontal`** に変更
6. `Fill Origin` を **`Left`** に変更
7. `Fill Amount` を **`1`** に設定（初期値：満タン）

> ⚠️ `Image Type` を Filled にしないと HP が減っても見た目が変わりません。必ず設定してください。

**HPText を作成する**

1. `HPArea` を右クリック → `UI > Text - TextMeshPro` → 名前を `HPText` に変更
2. Anchor を **Stretch / Stretch**（全面）に設定、Left/Right/Top/Bottom = `0`
3. Inspector の TextMeshPro コンポーネントを設定：
   - `Text` 欄に `12 / 12`（仮の表示、実行時にスクリプトが書き換える）
   - `Font Size` : `12`
   - `Alignment` : **Center / Middle**（水平・垂直とも中央）
   - `Color` : `#3D3833`

---

### 7-3. 弾数表示エリアの作成

弾数表示は画面右下に固定します。

#### GameObject の構成

```
HUD
└── AmmoArea
    ├── AmmoContainer      （弾アイコンを横並びに生成する親）
    ├── AmmoBulletPrefab   （弾アイコンのテンプレート、非表示）
    └── ReloadCircle       （リロード中に表示する円形プログレス、非表示）
```

#### 手順

**AmmoArea を作成する**

1. `HUD` を右クリック → `Create Empty` → 名前を `AmmoArea` に変更
2. `RectTransform` を以下に設定：

| 項目 | 値 |
|---|---|
| Anchor | **Bottom Right**（右下固定） |
| Pivot | `1, 0` |
| Pos X | `-16` |
| Pos Y | `16` |
| Width | `200` |
| Height | `40` |

**AmmoContainer を作成する**

1. `AmmoArea` を右クリック → `Create Empty` → 名前を `AmmoContainer` に変更
2. Anchor を **Stretch / Stretch**（全面）に設定
3. `Add Component` → **`Horizontal Layout Group`** を追加
   - `Spacing` : `4`
   - `Child Alignment` : **Middle Right**
   - `Control Child Size` : Width・Height ともに OFF
   - `Child Force Expand` : Width・Height ともに OFF

**AmmoBulletPrefab を作成する**

1. `AmmoArea` を右クリック → `UI > Image` → 名前を `AmmoBulletPrefab` に変更
2. サイズ: Width `16`、Height `16`
3. `Color` を `#3D3833` に設定
4. Sprite は弾丸アイコンがあればアサイン、なければ `UI/Default`（白い四角）で仮置きでOK
5. Inspector 上部のチェックボックス（GameObject の有効/無効）を **OFF（非表示）** にする

> `AmmoBulletPrefab` は Instantiate のテンプレートとして使います。Hierarchy 上に置いたまま非表示にしてください。

**ReloadCircle を作成する**

1. `AmmoArea` を右クリック → `UI > Image` → 名前を `ReloadCircle` に変更
2. サイズ: Width `32`、Height `32`
3. Anchor を **Middle Right**（中央右）に設定
4. `Image Type` を **`Filled`** に変更
5. `Fill Method` を **`Radial 360`** に変更
6. `Fill Origin` を **`Top`** に変更
7. `Fill Amount` を **`0`** に設定（初期値：空）
8. `Color` を `#7A766F` に設定
9. Inspector 上部のチェックボックスを **OFF（非表示）** にする

---

### 7-4. GameHUD スクリプトへの参照アサイン

`HUD` オブジェクトの `GameHUD` コンポーネントを選択して、各フィールドに参照をドラッグします。

| Inspector のフィールド名 | アサインするもの | 場所 |
|---|---|---|
| `Tank Status` | `TankStatus` コンポーネント | `Tank` GameObject |
| `Bullet Manager` | `TankBulletManager` コンポーネント | `Tank` GameObject |
| `Theme` | `ThemeColor.asset` | `Assets/ProjectTanker/Data/` |
| `Hp Fill` | `HPFill` の **Image コンポーネント** | `HUD/HPArea/HPFill` |
| `Hp Text` | `HPText` の **TextMeshProUGUI コンポーネント** | `HUD/HPArea/HPText` |
| `Ammo Container` | `AmmoContainer` の **Transform** | `HUD/AmmoArea/AmmoContainer` |
| `Ammo Bullet Prefab` | `AmmoBulletPrefab` の **Image コンポーネント** | `HUD/AmmoArea/AmmoBulletPrefab` |
| `Reload Circle` | `ReloadCircle` の **Image コンポーネント** | `HUD/AmmoArea/ReloadCircle` |

> **コンポーネントをドラッグするには**: Hierarchy でオブジェクトを選択して Inspector を開き、目的のコンポーネント名（例：`Image`）の左のアイコン部分を、GameHUD の対応フィールドへドラッグします。

---

### 7-5. シーンを保存

`Ctrl + S` でシーンを保存してください。

---

## 8. 動作確認・テスト手順

ここからはプレイモードに入って実際の動作を確認します。  
各項目を上から順番にテストしてください。

---

### 8-1. 起動時の表示確認

1. Unity エディタ上部の **▶ ボタン**を押してプレイモードを開始
2. 以下を目視確認する

| 確認項目 | 期待される表示 |
|---|---|
| 画面全体の背景色 | クリーム色（`#F0EDE8`）になっている |
| 左上 | HP バーが表示されている（コーラルレッドのバー） |
| 右下 | 弾数アイコンが `TankData` の `magazineCapacity` の数だけ横に並んでいる |
| 画面下部中央 | 7つのスロットが横一列に表示されている |
| 画面中央 | モジュール3択カードが表示されている |

**うまくいかない場合**:  
→ HP バーが出ない：`GameHUD` の `Hp Fill` フィールドに `HPFill` がアサインされているか確認  
→ 弾数アイコンが出ない：`Ammo Bullet Prefab` フィールドに `AmmoBulletPrefab` の **Image** がアサインされているか確認（GameObject ではなく Image コンポーネント）

---

### 8-2. モジュール3択 UI のテスト

ゲーム開始直後に3択カードが自動表示されます（`TankPresenter` がデバッグ用に `ModuleEarn()` を呼んでいるため）。

**確認手順**

1. プレイモード開始 → 画面中央に3枚のカードが表示されるのを確認
2. 各カード上部に色のついたバー（AccentBar）が表示されているか確認
   - モジュールの属性が `None` の場合はグレー（`#B0ABA3`）で表示される
3. いずれか1枚のカードをクリックする
   - カードパネルが閉じることを確認
   - E キーを押してインベントリを開き、選んだモジュールが一覧に追加されていることを確認

**確認チェック**
- [ ] 3択カードが表示される
- [ ] カード上部に AccentBar の色が表示される
- [ ] クリックでパネルが閉じてインベントリに追加される

---

### 8-3. インベントリ UI のテスト

**確認手順**

1. **E キー** を押してインベントリパネルが開くことを確認
2. 追加したモジュールのアイテム行が表示されているか確認
   - アイコン・モジュール名・右端の属性カラードットが表示されているか確認
3. **E キー** をもう一度押してパネルが閉じることを確認

**ドラッグ&ドロップのテスト**

1. E キーでインベントリを開く
2. アイテム行をドラッグ開始 → カーソルに半透明（70%）のゴーストアイコンが追従することを確認
3. そのまま画面下部のいずれかのスロットへドラッグしてドロップする
   - スロットにアイコンが表示されること
   - スロット下部の AccentLine に属性カラーが表示されること

**確認チェック**
- [ ] E キーでパネルが開閉する
- [ ] アイテム行に属性ドットが表示される
- [ ] ドラッグ中にゴーストが半透明で追従する
- [ ] スロットへドロップで装備できる
- [ ] スロットの AccentLine に属性色が出る

---

### 8-4. スロット → インベントリ への取り外しテスト

**確認手順**

1. スロットに装備済みのモジュールアイコンをドラッグ開始
2. インベントリパネルを E キーで開いておき、そのままパネル上へドロップする
   - スロットからモジュールが消えること
   - インベントリにモジュールが戻ること

**確認チェック**
- [ ] スロット→インベントリへのドラッグ&ドロップで取り外せる

---

### 8-5. HP バーのテスト

現時点で HP を手動で減らすトリガーがない場合は、スクリプトに一時的なデバッグコードを追加して確認します。

**`TankPresenter.cs` の `Start()` 末尾に1行追加（デバッグ用）**

```csharp
// デバッグ: HP を半分に減らして表示を確認
tankStatus.DealDamage(tankStatus.getMaxHP.Value / 2);
```

**確認手順**

1. 上記コードを追加してプレイモードを開始
2. HP バーが半分の長さになっているか確認
3. バーの色が `#E07A5F`（コーラル）から `#F2CC6A`（イエロー）の中間色になっているか確認

**確認後は追加したデバッグコードを必ず削除してください。**

**確認チェック**
- [ ] HP が減るとバーが縮まる
- [ ] HP 残量でバーのグラデーション色が変わる
- [ ] `HPText` に `現在HP / 最大HP` が表示される

---

### 8-6. 弾数・リロードのテスト

**確認手順**

1. プレイモードを開始し、右下の弾数アイコンの初期状態を確認（全アイコンが濃色）
2. **Space キー** を押して弾を発射する
   - 発射するたびにアイコンが右から薄色（`#CFC9C2`）に変わることを確認
3. 弾を撃ち切る（全アイコンが薄色になる）
   - `ReloadCircle`（円形プログレス）が表示されることを確認
   - リロード完了とともに円が消え、アイコンが1つずつ濃色に戻ることを確認

**確認チェック**
- [ ] 発射でアイコンが薄くなる
- [ ] 弾切れで ReloadCircle が出現する
- [ ] リロード完了でアイコンが復活する
- [ ] ReloadCircle がリロード進捗に合わせて塗りつぶされる

---

### 8-7. 全体の見た目の最終チェック

プレイモードでゲームを操作しながら以下を確認します。

- [ ] 背景・パネルすべてがクリーム〜グレージュの暖かいトーンに統一されている
- [ ] 白すぎる部分、黒すぎる部分がない
- [ ] 文字がすべて日本語フォントで表示されている（文字化け・豆腐文字がない）
- [ ] 全体的にゲームを遊んでいて目に優しい雰囲気になっている

---

## 9. トラブルシューティング

### スクリプトが Inspector に表示されない
Console ウィンドウ（`Ctrl+Shift+C`）でコンパイルエラーを確認してください。  
エラーがなければ Unity のコンパイルが終わるまで数秒待ってから再試行します。

### ThemeColor が Create メニューに出ない
スクリプトがまだコンパイルされていない状態です。Console にエラーがなければ少し待ってから再試行してください。

### HP バーが表示されない・動かない
- `GameHUD` の `Hp Fill` フィールドに `HPFill` オブジェクトの **Image コンポーネント** がアサインされているか確認
- `HPFill` の `Image Type` が **Filled** / `Fill Method` が **Horizontal** になっているか確認

### 弾数アイコンが出ない
- `Ammo Bullet Prefab` フィールドに `AmmoBulletPrefab` の **Image コンポーネント** がアサインされているか確認（GameObjectごとではなくコンポーネント単位でドラッグ）
- `AmmoBulletPrefab` が非表示（チェックOFF）になっているか確認

### ReloadCircle が出ない・円にならない
- `Image Type` が **Filled**、`Fill Method` が **Radial 360** になっているか確認
- Sprite に**円形のスプライト**が設定されているか確認（四角のスプライトでは円に見えません）

### ドラッグ&ドロップが反応しない
- Hierarchy に `EventSystem` が存在するか確認  
  ない場合: `GameObject > UI > Event System` で追加
- ドラッグ元・ドロップ先の GameObject に `Raycast Target` が ON になっているか確認

### 属性カラー（AccentBar / AccentLine / ElementDot）が表示されない
- 各スクリプトの Inspector で `Theme` フィールドに `ThemeColor.asset` がアサインされているか確認
- モジュールの `moduleElement` が `None` に設定されている場合はグレー（`#B0ABA3`）で表示されます（正常な動作です）

### フォントが豆腐文字（□）になる
- `Edit > Project Settings > TextMesh Pro` の `Default Font Asset` に日本語対応の Font Asset が設定されているか確認
- Dynamic モードの Font Asset を使っている場合、エディタ上では初回表示時に一瞬豆腐になることがあります。プレイモードを再起動すると解消します。
