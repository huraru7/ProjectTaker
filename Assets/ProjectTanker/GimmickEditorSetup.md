# ギミック回路 Unity Editor セットアップガイド

## 準備：SignalChannel アセットの作成

回路の「電線」にあたる SignalChannel は ScriptableObject。使うチャンネルの数だけ作成する。

```
Project ウィンドウで右クリック
→ Create > Gimmick > SignalChannel
→ 名前を付けて保存（例: ChannelA.asset、ChannelDoor.asset）
```

チャンネルは複数の送信側・受信側で共有できる。1つの信号で複数のドアを開けたい場合は同じ .asset をセットすればよい。

Scene ビューでボタンオブジェクトを選択すると、接続先のドアへの線とチャンネルカラーの円がGizmosとして表示される。

---

## パターン1：ボタンを撃ってドアを開ける

```
GimmickButton ──→ ChannelA ──→ GimmickDoor
```

### ① ChannelA.asset を作成

### ② ボタンオブジェクトを作る

GimmickButton は「台座（ベース）」と「押し込みパーツ（ButtonTop）」の2層構造。
弾が ButtonTop に当たるとパーツが沈み込んでドアへ信号を送る。

```
Button (GameObject)
├── GimmickButton.cs
├── Base (子 GameObject)
│   └── SpriteRenderer（壁と同じマテリアル・横幅広め）
└── ButtonTop (子 GameObject)
    ├── SpriteRenderer（押し込みパーツ・横幅狭め）
    ├── CircleCollider2D ← Is Trigger = TRUE
    └── SwitchPart.cs
```

**作成手順**

1. `Create Empty` → 名前 **`Button`**、`GimmickButton` をアタッチ
2. 子 **`Base`** を作成
   - `SpriteRenderer` をアタッチ（横に広いスプライト）
   - マテリアルを壁と同じ `WallTile.mat` にすると見た目が統一される
3. 子 **`ButtonTop`** を作成
   - `SpriteRenderer` をアタッチ（`Base` より横幅が狭いスプライト）
   - `CircleCollider2D` をアタッチ → **Is Trigger にチェック**
   - `SwitchPart` をアタッチ
4. `GimmickButton` Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Channel | ChannelA.asset |
| Button Top | `ButtonTop` の Transform |
| Top Renderer | `ButtonTop` の SpriteRenderer |
| Off Color | 押していない時の色（デフォルト: 黄） |
| On Color | 押した時の色（デフォルト: 緑） |
| Reset Delay | **0 = 永続ON** / **0より大きい値 = 指定秒後にOFFへ戻る** |
| Press Depth | 沈み込む量（デフォルト: 0.15） |

### ③ ドアオブジェクトを作る

1. `Create Empty` → 名前 **`Door`**、`GimmickDoor` をアタッチ
2. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Channel | ChannelA.asset（ボタンと同じ） |
| Tilemap | ドアのセルが乗っている Tilemap |
| Door Tile | ドアとして使うタイルアセット |
| Cell Position | ドアセルの座標（Tilemap 上の整数座標） |

> **Cell Position の調べ方**: Scene ビューでタイルをクリック → Inspector の「Cell」座標を読む。

### 動作モード

| Reset Delay | 動作 |
|---|---|
| `0` | 弾が当たったらそのまま永続ON（ドアが開きっぱなし） |
| `2` | 弾が当たってから2秒後にOFFに戻る（ドアが閉まる） |

---

## 新しいギミックを追加するとき

### 送信側を増やす

```csharp
public class GimmickXxx : SignalSender
{
    void SomeCondition()
    {
        Send(true);   // ON を送る
        Send(false);  // OFF を送る
    }
}
```

`SignalSender` を継承して `Send(bool)` を呼ぶだけ。Channel フィールドと Gizmos は基底クラスが持っている。

### 受信側を増やす

```csharp
public class GimmickYyy : SignalReceiver
{
    protected override void OnReceive(bool value)
    {
        // value が true なら ON、false なら OFF
    }
}
```

`SignalReceiver` を継承して `OnReceive` を実装するだけ。購読の登録・解除は基底クラスが自動で行う。

---

## スクリプトの格納場所

```
Assets/ProjectTanker/Script/Gimmick/
├── SignalChannel.cs      （ScriptableObject・電線）
├── SignalSender.cs       （送信側の基底クラス）
├── SignalReceiver.cs     （受信側の基底クラス）
├── Senders/
│   ├── GimmickButton.cs  （弾で押すボタン）
│   └── SwitchPart.cs     （ButtonTop の当たり判定）
└── Receivers/
    └── GimmickDoor.cs    （Tilemap のタイルを開閉）
```
