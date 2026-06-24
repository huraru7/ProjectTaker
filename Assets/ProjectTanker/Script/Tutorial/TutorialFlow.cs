using System.Collections;
using R3;
using UnityEngine;

/// <summary>
/// チュートリアル全体を制御する。Start() でコルーチンを起動し、
/// 各ステップを順番に実行する。同一シーン内で動作し、完了後はチュートリアル用
/// オブジェクトを非アクティブ化して通常ゲームプレイへ移行する。
/// </summary>
public class TutorialFlow : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private TankStatus         playerStatus;
    [SerializeField] private TankBulletManager  playerBulletManager;
    [SerializeField] private Transform          playerTransform;

    [Header("Tutorial Objects")]
    [SerializeField] private Transform     moveTarget;            // 移動チュートリアルの目標地点
    [SerializeField] private TankStatus    enemyStatus;           // 倒す敵（反射を駆使する位置に配置）
    [SerializeField] private SignalChannel doorSignal;
    [SerializeField] private Transform     goalMarker;
    [SerializeField] private float         targetRadius = 1.5f;   // 各目標地点の到達判定半径

    [Tooltip("チュートリアル完了後に非アクティブ化するオブジェクト（ドア・ボタン・ゴールマーカーなど）")]
    [SerializeField] private GameObject[] tutorialOnlyObjects;

    void Start() => StartCoroutine(RunTutorial());

    private IEnumerator RunTutorial()
    {
        if (enemyStatus != null)
            enemyStatus.gameObject.SetActive(false);

        // 1. ようこそ
        ShowTutorial("ようこそ「Tanker」へ！\n壁反射を活かした弾幕構築ゲームです。");
        yield return new WaitForSecondsRealtime(5f);

        // 2. 移動：指定地点まで移動する
        if (moveTarget != null)
        {
            ShowTutorial("WASD キーで戦車を移動しよう！\n光っている地点まで移動してみよう。");
            yield return new WaitUntil(() =>
                Vector3.Distance(playerTransform.position, moveTarget.position) < targetRadius);
            moveTarget.gameObject.SetActive(false);
        }

        // 3. 射撃
        int prevRounds = playerBulletManager.getTotalRounds.Value;
        ShowTutorial("マウスで砲台を向けて、スペースキーで射撃！");
        yield return new WaitUntil(() =>
            playerBulletManager.getTotalRounds.Value < prevRounds);

        // 4. 反射を使って敵を倒す
        if (enemyStatus != null)
        {
            enemyStatus.gameObject.SetActive(true);
            bool enemyDead = false;
            enemyStatus.OnDead
                .Subscribe(_ => enemyDead = true)
                .AddTo(this);
            ShowTutorial("敵が現れた！\n壁に向けて撃つと弾が反射する。緑の予告線を活用しよう！");
            yield return new WaitUntil(() => enemyDead);
            NotificationManager.Success("敵を撃破！");
            yield return new WaitForSecondsRealtime(1f);
        }

        // 5. ギミックドア
        if (doorSignal != null)
        {
            ShowTutorial("ボタンに弾を当てるとドアが開く！\n反射を使えば奥の目標も狙えるぞ。");
            yield return new WaitUntil(() => doorSignal.IsOn);
            NotificationManager.Success("ドアが開いた！");
            yield return new WaitForSecondsRealtime(1f);
        }

        // 6. ゴールへ移動
        if (goalMarker != null)
        {
            ShowTutorial("ゴール地点まで進もう！");
            yield return new WaitUntil(() =>
                Vector3.Distance(playerTransform.position, goalMarker.position) < targetRadius);
        }

        // 完了 → チュートリアル用オブジェクトを片付けてゲーム開始
        NotificationManager.Success("チュートリアル完了！ステージ1へ進め！");
        yield return new WaitForSecondsRealtime(1.5f);

        foreach (var obj in tutorialOnlyObjects)
            if (obj != null) obj.SetActive(false);

        gameObject.SetActive(false);
    }

    private void ShowTutorial(string msg)
    {
        NotificationManager.Show(new NotificationData
        {
            Message     = msg,
            AccentColor = new Color(0.961f, 0.773f, 0.094f),
            Duration    = 30f,
        });
    }
}
