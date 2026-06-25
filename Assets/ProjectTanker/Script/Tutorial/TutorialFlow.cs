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
    [SerializeField] private TankStatus playerStatus;
    [SerializeField] private TankBulletManager playerBulletManager;
    [SerializeField] private Transform playerTransform;

    [Header("Move Targets")]
    [SerializeField] private Transform moveTarget1;  // ステップ2：最初の移動先
    [SerializeField] private Transform moveTarget2;  // ステップ3と4の間
    [SerializeField] private Transform moveTarget3;  // ステップ4と5の間
    [SerializeField] private Transform moveTarget4;  // ステップ6と7の間

    [Header("Enemies")]
    [SerializeField] private TankStatus directShotEnemy; // ステップ3：直接射撃で倒す敵
    [SerializeField] private TankStatus bounceEnemy;     // ステップ4：反射で倒す敵
    [SerializeField] private TankStatus gimmickEnemy;    // ステップ5：ギミックを使って倒す敵

    [Header("Gimmick & Goal")]
    [SerializeField] private SignalChannel doorSignal;   // ステップ7：ドアのシグナルチャンネル
    [SerializeField] private Transform goalMarker;   // ステップ8：ゴール地点
    [SerializeField] private float targetRadius = 1.5f;

    [Header("Module")]
    [SerializeField] private TankModuleManager tankModuleManager;

    [Tooltip("チュートリアル完了後に非アクティブ化するオブジェクト")]
    [SerializeField] private GameObject[] tutorialOnlyObjects;

    void Start()
    {
        if (directShotEnemy != null) directShotEnemy.gameObject.SetActive(false);
        if (bounceEnemy != null) bounceEnemy.gameObject.SetActive(false);
        if (gimmickEnemy != null) gimmickEnemy.gameObject.SetActive(false);

        StartCoroutine(RunTutorial());
    }

    private IEnumerator RunTutorial()
    {
        // 1. ようこそ
        ShowTutorial("ようこそ「Tanker」へ！\n壁反射を活かした弾幕構築ゲームです。");
        yield return new WaitForSecondsRealtime(5f);

        // 2. 移動
        yield return MovePhase(moveTarget1, "WASD キーで戦車を移動しよう！\n光っている地点まで移動してみよう。");

        // 3. 直接射撃で敵を倒す
        if (directShotEnemy != null)
        {
            directShotEnemy.gameObject.SetActive(true);
            bool dead = false;
            directShotEnemy.OnDead.Subscribe(_ => dead = true).AddTo(this);
            ShowTutorial("マウスで砲台を向けて、スペースキーで射撃！\n敵を倒そう!");
            yield return new WaitUntil(() => dead);
            NotificationManager.Success("敵を撃破！");
            yield return new WaitForSecondsRealtime(1f);
        }

        // 3→4 移動
        yield return MovePhase(moveTarget2, "次のエリアへ進もう！");

        // 4. 反射を使って敵を倒す
        if (bounceEnemy != null)
        {
            bounceEnemy.gameObject.SetActive(true);
            bool dead = false;
            bounceEnemy.OnDead.Subscribe(_ => dead = true).AddTo(this);
            ShowTutorial("壁に向けて撃つと弾が反射する!\n緑の予告線を活用して敵を倒そう!");
            yield return new WaitUntil(() => dead);
            NotificationManager.Success("反射で敵を撃破！");
            yield return new WaitForSecondsRealtime(1f);
        }

        // 4→5 移動
        yield return MovePhase(moveTarget3, "次のエリアへ進もう！");

        // 5. ギミックを使って敵を倒す
        if (gimmickEnemy != null)
        {
            gimmickEnemy.gameObject.SetActive(true);
            bool dead = false;
            gimmickEnemy.OnDead.Subscribe(_ => dead = true).AddTo(this);
            ShowTutorial("ボタンに弾を当てるとギミックが作動する！\nうまく利用して敵を倒そう！");
            yield return new WaitUntil(() => dead);
            NotificationManager.Success("ギミックで撃破！");
            yield return new WaitForSecondsRealtime(1f);
        }

        // 6. モジュール説明
        if (tankModuleManager != null)
        {
            ShowTutorial("レベルアップするとモジュールを選択できる！\n能力をカスタマイズしてタンクを強化しよう。");
            yield return new WaitForSecondsRealtime(2.5f);

            bool moduleSelected = false;
            tankModuleManager.OnSlotsChanged
                .Subscribe(_ => moduleSelected = true)
                .AddTo(this);
            tankModuleManager.ModuleEarn();

            yield return new WaitUntil(() => moduleSelected);
            NotificationManager.Success("モジュールを装備！");
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // 6→7 移動
        yield return MovePhase(moveTarget4, "次のエリアへ進もう！");

        // 7. ドアギミック
        if (doorSignal != null)
        {
            ShowTutorial("ボタンに弾を当てるとドアが開く！\n反射を使えば奥の目標も狙えるぞ。");
            yield return new WaitUntil(() => doorSignal.IsOn);
            NotificationManager.Success("ドアが開いた！");
            yield return new WaitForSecondsRealtime(1f);
        }

        // 8. ゴールへ移動
        if (goalMarker != null)
        {
            ShowTutorial("ゴール地点まで進もう！");
            yield return new WaitUntil(() =>
                Vector3.Distance(playerTransform.position, goalMarker.position) < targetRadius);
        }

        // 完了
        NotificationManager.Success("チュートリアル完了!ステージ1へ進め!");
        yield return new WaitForSecondsRealtime(1.5f);

        foreach (var obj in tutorialOnlyObjects)
            if (obj != null) obj.SetActive(false);

        gameObject.SetActive(false);
    }

    private IEnumerator MovePhase(Transform target, string message)
    {
        if (target == null) yield break;
        ShowTutorial(message);
        OffScreenIndicator.Instance?.Register(target, new Color(0.961f, 0.773f, 0.094f));
        yield return new WaitUntil(() =>
            Vector3.Distance(playerTransform.position, target.position) < targetRadius);
        OffScreenIndicator.Instance?.Unregister(target);
        target.gameObject.SetActive(false);
    }

    private void ShowTutorial(string msg)
    {
        NotificationManager.Show(new NotificationData
        {
            Message = msg,
            AccentColor = new Color(0.961f, 0.773f, 0.094f),
            Duration = 30f,
        });
    }
}
