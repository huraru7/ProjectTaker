# Unity エディタ作業ガイド — project Tanker

---

## モジュール3択UI ビジュアル刷新（説明文＋リロール機能）

スクリプトはすべて実装済み。`ModuleOptionButton` への参照セットと、UI階層への要素追加が必要。

---

## ① ModuleOptionButton プレハブへの要素追加

各カードに Divider（区切り線）と DescriptionText（説明文）を追加する。

```
OptionButton
├── AccentBar（上部の細い属性カラーバー：既存）
├── IconImage（アイコン、中央：既存）
├── NameText（モジュール名、中央太字：既存）
├── Divider（新規：細い横線 Image、NameTextの下）
└── DescriptionText（新規：説明文、2行、中央揃え小さめフォント）
```

作成後、`ModuleOptionButton` コンポーネントの **Description Text** フィールドに `DescriptionText` をセットする（3枚すべてのカードで実施）。

---

## ② パネルのタイトル装飾

`GetModuleSelectUI` の `panel` 上部に、静的テキスト「アップグレードを選択」と左右の装飾的なダッシュ（"···─" 等）を追加する。コード参照は不要（見た目のみ）。

---

## ③ Reroll 行の追加

`GetModuleSelectUI` の `panel` 内、カード群の下に以下を追加する。

```
RerollRow
├── RKeyBadge（"R" と表示する小さな角丸 Image、装飾）
├── RerollCountText（"リロール (3)" 表示用 TextMeshProUGUI）
└── Button コンポーネント（RerollRow か親 GameObject に付与）
```

作成後、`GetModuleSelectUI` コンポーネントの Inspector で以下をセット：

| フィールド | 設定内容 |
|---|---|
| Reroll Button | 上記 Button コンポーネント |
| Reroll Count Text | `RerollCountText` |

---

## ④ ThemeColor の確認

`ThemeColor` アセットの `earth`/`wind`/`fire` が以下に近いか確認・調整する。

| Element | 色 |
|---|---|
| Earth | 緑 |
| Wind | 黄 |
| Fire | 赤 |

---

## ⑤ TankModuleManager の設定

Hierarchy でプレイヤータンクを選択 → `TankModuleManager` の **Max Reroll Count** を確認（デフォルト 3、好みで調整可）。

---

## 動作確認

| 確認 | 期待 |
|---|---|
| 3択UIを開く | 各カードに説明文が表示される、下部に「リロール (3)」ボタンが出る |
| リロールボタンをクリック | 3枚とも新しい候補に変わり、ポップイン演出が再生される。残数が「リロール (2)」になる |
| Rキーを押す | クリックと同じ効果でリロールされる |
| 残数が0になる | ボタンが非活性になり、Rキーを押しても何も起きない |
| 別のレベルアップで3択を再度開く | リロール残数は前回の続きから（リセットされない） |
