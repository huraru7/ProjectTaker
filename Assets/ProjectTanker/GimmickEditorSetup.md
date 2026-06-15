# ギミック回路 Unity Editor セットアップガイド

## 準備：SignalChannel アセットの作成

回路の「電線」にあたる SignalChannel は ScriptableObject。使うチャンネルの数だけ作成する。

```
Project ウィンドウで右クリック
→ Create > Gimmick > SignalChannel
→ 名前を付けて保存（例: ChannelA.asset、ChannelDoor.asset）
```

チャンネルは複数の送信側・受信側で共有できる。1つの信号で複数のドアを開けたい場合は同じ .asset をセットすればよい。

---

## パターン1：ボタン1つでドアを開ける（基本）

```
GimmickButton ──→ ChannelA ──→ GimmickDoor
```

### 手順

**① ChannelA.asset を作成**

**② ボタンオブジェクトを作る（ビジュアルスイッチ構成）**

GimmickButton は「ベース台座」＋「レバー」＋「LED インジケーター」の 3 層構造で作る。

```
Button (GameObject)
├── GimmickButton.cs
├── SpriteRenderer（台座スプライト）
│
├── Indicator (子 GameObject)
│   └── SpriteRenderer ← GimmickSwitch マテリアル割当（LED 発光ドット）
│
└── SwitchLever (子 GameObject)
    ├── SpriteRenderer（レバースプライト）
    ├── CircleCollider2D ← Is Trigger = TRUE
    └── SwitchPart.cs
```

**マテリアル作成（初回のみ）**
1. `Art/Materials/` 内で右クリック → Create > Material → 名前 **`GimmickSwitch`**
2. Shader を `ProjectTanker/GimmickSwitch` に変更

**Prefab 構築手順**
1. Hierarchy で `Create Empty` → 名前 **`Button`**
2. `GimmickButton` コンポーネントをアタッチ
3. 任意で台座用 `SpriteRenderer` を追加
4. 子オブジェクト **`Indicator`** を作成
   - `SpriteRenderer` をアタッチ
   - Material に `GimmickSwitch` をセット（小さな丸スプライト推奨）
5. 子オブジェクト **`SwitchLever`** を作成
   - `SpriteRenderer` をアタッチ（縦長の矩形など）
   - `CircleCollider2D` をアタッチ → **Is Trigger にチェック**
   - `SwitchPart` コンポーネントをアタッチ
6. `Button` ルートの `GimmickButton` Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Channel | ChannelA.asset |
| Send Once | 一度のみ反応させる場合は ON |
| Lever Transform | `SwitchLever` の Transform をドラッグ |
| Indicator Renderer | `Indicator` の SpriteRenderer をドラッグ |
| Lever On Angle | `-45`（左倒れ）または `45`（右倒れ） |
| Anim Duration | `0.2`（秒） |

> **Physics 2D 確認**: Bullet の Layer と SwitchLever の Layer が `Project Settings → Physics 2D → Layer Collision Matrix` で交差が有効になっていること（既存ボタンと同 Layer なら変更不要）。

**③ ドアオブジェクトを作る**
1. 空の GameObject を作成 → 名前「Door」
2. `GimmickDoor` コンポーネントを追加
3. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Channel | ChannelA.asset |
| Tilemap | ドアのセルが乗っている Tilemap |
| Door Tile | ドアとして使うタイルアセット |
| Cell Position | ドアセルの座標（Tilemap 上の整数座標） |

> **Cell Position の調べ方**: Scene ビューでタイルをクリックし、Tilemap の Inspector に表示される「Cell」座標を読む。

---

## パターン2：ボタン2つ同時押しでドアを開ける（AND 回路）

```
ButtonA ──→ ChannelA ──┐
                        ├──→ SignalGate(AND) ──→ ChannelResult ──→ Door
ButtonB ──→ ChannelB ──┘
```

### 手順

**① ChannelA.asset、ChannelB.asset、ChannelResult.asset を作成**

**② ButtonA / ButtonB を上記パターン1と同様に作成**（Channel はそれぞれ ChannelA / ChannelB）

**③ Gate オブジェクトを作る**
1. 空の GameObject を作成 → 名前「Gate_AND」
2. `SignalGate` コンポーネントを追加
3. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Gate Type | AND |
| Inputs（Size=2） | [0] ChannelA.asset / [1] ChannelB.asset |
| Output | ChannelResult.asset |

**④ Door を作成**（Channel は ChannelResult.asset）

OR 回路にしたい場合は Gate Type を `OR` に変えるだけ。

---

## パターン3：一定時間だけドアが開く（タイマー付き）

```
Button ──→ ChannelA ──→ SignalTimer(3秒) ──→ ChannelA ──→ Door
```

SignalTimer は ChannelA を監視して、ON を受け取ったら指定秒後に自動で OFF を送る。

### 手順

**① ChannelA.asset を作成**

**② Timer オブジェクトを作る**
1. 空の GameObject を作成 → 名前「Timer」
2. `SignalTimer` コンポーネントを追加
3. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Channel | ChannelA.asset |
| Duration | 3（秒） |

**③ Button / Door を作成**（どちらも ChannelA.asset を使用）

---

## パターン4：押すたびに開閉が切り替わるトグル式ドア

```
Button ──→ ChannelA ──→ SignalToggle ──→ ChannelResult ──→ Door
```

### 手順

**① ChannelA.asset、ChannelResult.asset を作成**

**② Toggle オブジェクトを作る**
1. 空の GameObject を作成 → 名前「Toggle」
2. `SignalToggle` コンポーネントを追加
3. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Input | ChannelA.asset |
| Output | ChannelResult.asset |

**③ Button**（ChannelA.asset）/ **Door**（ChannelResult.asset）を作成

---

## パターン5：圧力板でタンクが通過したときだけ開く壁

```
GimmickPressure ──→ ChannelA ──→ GimmickWall
```

`GimmickPressure` はプレイヤーまたは敵タンクが乗ると ON、離れると OFF を送る。

### 手順

**① Pressure オブジェクトを作る**
1. 空の GameObject を作成 → 名前「PressurePlate」
2. `GimmickPressure` コンポーネントを追加
3. `Collider2D` を追加して **Is Trigger にチェック**
4. Channel に ChannelA.asset をセット

**② Wall オブジェクトを作る**
1. 壁用の GameObject を作成（SpriteRenderer + Collider2D 付き）
2. `GimmickWall` コンポーネントを追加
3. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Channel | ChannelA.asset |
| Wall Collider | 同じオブジェクトの Collider2D をドラッグ |
| Wall Renderer | 同じオブジェクトの SpriteRenderer をドラッグ |

> ON 受信で Collider2D・SpriteRenderer が無効化される（通行可能に）。OFF で元に戻る。

---

## パターン6：弾を反射させる鏡ギミック

```
Button ──→ ChannelA ──→ GimmickMirror
```

### 手順

**① Mirror オブジェクトを作る**
1. 反射板の GameObject を作成（SpriteRenderer + Collider2D 付き）
2. `GimmickMirror` コンポーネントを追加
3. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| Channel | ChannelA.asset |
| Angle Off | OFF 時の Z 角度（例: 45） |
| Angle On | ON 時の Z 角度（例: -45） |
| Rotate Speed | 回転速度 deg/sec（例: 90） |

---

## 回路の可視化：SignalWire

送信側と受信側の間に線を引いて、信号の ON/OFF を黄色/グレーで表示する。

### 手順

1. 空の GameObject を作成 → 名前「Wire」
2. `LineRenderer` コンポーネントを追加
3. `SignalWire` コンポーネントを追加
4. Inspector で以下をセット

| フィールド | 設定内容 |
|---|---|
| From | 送信側オブジェクトの Transform |
| To | 受信側オブジェクトの Transform |
| Channel | 対象の .asset |

> 信号 ON で黄色（#F5C518）、OFF でグレー（#5D6470）に変化する。

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

`SignalSender` を継承して `Send(bool)` を呼ぶだけ。Channel フィールドは基底クラスが持っている。

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
├── SignalChannel.cs
├── SignalSender.cs
├── SignalReceiver.cs
├── SignalGate.cs
├── SignalTimer.cs
├── SignalToggle.cs
├── SignalWire.cs
├── Senders/
│   ├── GimmickButton.cs
│   └── GimmickPressure.cs
└── Receivers/
    ├── GimmickDoor.cs
    ├── GimmickMirror.cs
    └── GimmickWall.cs
```
