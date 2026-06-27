# Unity エディタ作業ガイド — project Tanker

---

## AreaExplosion プレハブの作成（反射爆発・将来の爆弾で共通利用）

スクリプト・シェーダーはすべて実装済み。マテリアル・プレハブの作成と Inspector へのアサインが必要。

---

## ① AreaExplosion マテリアルの作成

`Assets/ProjectTanker/Art/Material/` に新規マテリアルを作成し、Shader を `ProjectTanker/AreaExplosion` に設定する。

| プロパティ | 値 |
|---|---|
| Color | R:1.0 G:0.45 B:0.10 A:1（暖色オレンジ） |
| Alpha | 1.0（コードがアニメーションするため初期値のまま） |
| Ring Width | 0.07 |
| HDR Intensity | 2.0 |

---

## ② AreaExplosion プレハブの作成

空の GameObject を作成し、以下のコンポーネントを追加する。

```
AreaExplosion (GameObject)
├── MeshFilter     … Mesh = Quad（Unity ビルトイン）
├── MeshRenderer   … Material = AreaExplosion.mat
└── AreaExplosion  … Script（自動削除・範囲ダメージ）
```

- `Transform.localScale` は **(0, 0, 0)** に設定（スクリプトが拡大アニメーションを制御する）
- MeshRenderer の **Sorting Layer / Order in Layer** を弾（Bullet）より手前に設定
- 作成したら `Assets/ProjectTanker/Art/Prefab/` などへ Prefab 化して保存

---

## ③ SplitShotEffect アセットの Inspector 更新

Hierarchy または Project からプレイヤータンクにアタッチされている `SplitShotEffect` ScriptableObject を開き、以下をセットする。

| フィールド | 設定内容 |
|---|---|
| Explosion Prefab | 上記で作成した **AreaExplosion プレハブ** |
| Base Radius | 2.0 |
| Base Damage | 5 |

---

## ④ モジュール3択UI（前回の未完了タスク）

### カード内レイアウト（ModuleOptionButton プレハブ）

```
OptionButton
├── AccentBar（既存）
├── IconImage（既存）
├── NameText（既存）
├── Divider（新規：細い横線 Image）
└── DescriptionText（新規：2行、中央揃え小フォント → descriptionText フィールドにセット）
```

### パネル上部にタイトルを追加

「アップグレードを選択」と装飾ダッシュを静的テキストとして追加（コード不要）。

### パネル下部に Reroll 行を追加

```
RerollRow
├── RKeyBadge（"R" と表示する装飾 Image）
├── RerollCountText（TextMeshProUGUI → GetModuleSelectUI の Reroll Count Text にセット）
└── Button（GetModuleSelectUI の Reroll Button にセット）
```

---

## 動作確認

| 確認項目 | 期待挙動 |
|---|---|
| 弾が壁に反射する | 反射点にオレンジのリングが出現し 0.4 秒で拡大・消滅する |
| リング範囲内に敵がいる | 即時ダメージが入る |
| リング範囲外の敵 | ダメージなし |
| 弾の継続 | そのまま反射を続ける（毎反射で爆発が発生） |
| スタック 2 | 範囲が +0.5 拡大、ダメージが 2 倍 |
| カメラシェイク | 爆発のたびに軽くシェイクする |

---

## タイトル画面セットアップ（UI Toolkit）

スクリプト・UXML・USSはすべて実装済み。以下の手順のみが必要。

### ① Panel Settings の作成

Project ウィンドウで右クリック →  
**Create > UI Toolkit > Panel Settings Asset**  
→ `Assets/ProjectTanker/UI/TitlePanelSettings.asset` として保存

| プロパティ | 値 |
|---|---|
| Scale Mode | Scale With Screen Size |
| Reference Resolution | 1920 × 1080 |
| Match | 0.5（Width と Height の中間） |

### ② UXML に USS をリンク

Project ウィンドウで `Assets/ProjectTanker/UI/TitleDocument.uxml` をダブルクリック → UI Builder が開く → **StyleSheets パネル**（左上）で `+` → `TitleDocument.uss` を追加して保存。

### ③ シーンへの配置（`0_title.unity` を開いた状態で）

1. Hierarchy で空の GameObject を作成 → 名前を `TitleUI` にする
2. `UIDocument` コンポーネントを追加
   - **Panel Settings** → 上記で作成した `TitlePanelSettings` をセット
   - **Source Asset** → `Assets/ProjectTanker/UI/TitleDocument.uxml` をセット
3. `TitleUI` スクリプトを同じ GameObject にアタッチ
4. `TitleUI` コンポーネントのフィールドに以下をセット：
   - **Tank Body Sprite** → `Assets/ProjectTanker/Art/Image/tank/tank_body.png`
   - **Tank Turret Sprite** → `Assets/ProjectTanker/Art/Image/tank/tank_turret.png`
   - **Bullet Sprite** → `Assets/ProjectTanker/Art/Image/tank_bullet.png`（任意・未設定でも黄色の円で代替）

### ④ 動作確認

| 確認項目 | 期待挙動 |
|---|---|
| 再生直後 | 明るいブルーグレー背景 + 動くグリッド線 |
| 左パネル | 半透明白パネル内にロゴとボタンが表示 |
| 右パネル | プレイヤー戦車（固定）と敵戦車（左右パトロール）が表示 |
| 数秒後 | 弾が発射されて壁で反射、敵に命中、ヒット演出 |
| ホバー | ボタンが右にスライド |
| START クリック | `1_MainGame` シーンへ遷移 |
