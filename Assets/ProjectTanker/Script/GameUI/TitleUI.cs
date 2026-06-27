using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TitleUI : MonoBehaviour
{
    [SerializeField] private Sprite _tankBodySprite;
    [SerializeField] private Sprite _tankTurretSprite;
    [SerializeField] private Sprite _bulletSprite;

    // UI要素
    private VisualElement _gridBg;
    private VisualElement _logoArea;
    private Button        _startBtn;
    private Button        _creditBtn;

    // 戦車デモ要素
    private VisualElement _tankScene;
    private VisualElement _tankPlayer;
    private VisualElement _tankTurretPlayer;
    private VisualElement _tankEnemy;
    private VisualElement _tankTurretEnemy;
    private VisualElement _bulletEl;

    // グリッド
    private float _gridOffsetX;
    private float _gridOffsetY;
    private const float GridCellSize = 52f;
    private const float GridSpeed    = 22f;

    // ボタンホバー
    private float _startBtnX, _creditBtnX;
    private float _startBtnTargetX, _creditBtnTargetX;
    private const float BtnHoverDist = 16f;

    // 戦車座標（right-panel内のローカル座標）
    private float _sceneW, _sceneH;
    private float _playerX, _playerCY;
    private float _enemyBaseX, _enemyCY;
    private const float TankSize   = 88f;
    private const float TurretSize = 68f;
    private const float BulletSize = 14f;

    // 敵パトロール
    private float _enemyX;
    private float _enemyDir = 1f;
    private const float EnemyPatrolRange = 90f;
    private const float EnemySpeed       = 38f;

    // 弾
    private Vector2 _bulletPos, _bulletVel;
    private bool    _bulletActive;
    private bool    _bulletBounced;
    private float   _fireCooldown = 1.2f;
    private readonly List<Vector2> _trailPoints = new();

    // ヒット演出
    private float _enemyHitTimer;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _gridBg           = root.Q("grid-bg");
        _logoArea         = root.Q("logo-area");
        _startBtn         = root.Q<Button>("start-button");
        _creditBtn        = root.Q<Button>("credit-button");
        _tankScene        = root.Q("tank-scene");
        _tankPlayer       = root.Q("tank-player");
        _tankTurretPlayer = root.Q("tank-turret-player");
        _tankEnemy        = root.Q("tank-enemy");
        _tankTurretEnemy  = root.Q("tank-turret-enemy");
        _bulletEl         = root.Q("tank-bullet");

        // スプライト設定
        if (_tankBodySprite != null)
        {
            _tankPlayer.style.backgroundImage = new StyleBackground(_tankBodySprite);
            _tankEnemy.style.backgroundImage  = new StyleBackground(_tankBodySprite);
        }
        if (_tankTurretSprite != null)
        {
            _tankTurretPlayer.style.backgroundImage = new StyleBackground(_tankTurretSprite);
            _tankTurretEnemy.style.backgroundImage  = new StyleBackground(_tankTurretSprite);
        }
        if (_bulletSprite != null)
            _bulletEl.style.backgroundImage = new StyleBackground(_bulletSprite);

        // 描画コールバック
        _gridBg.generateVisualContent    += DrawGrid;
        _tankScene.generateVisualContent += DrawTrail;

        // ボタン
        _startBtn.clicked  += OnStart;
        _creditBtn.clicked += OnCredit;
        _startBtn.RegisterCallback<MouseEnterEvent>(_  => _startBtnTargetX  = BtnHoverDist);
        _startBtn.RegisterCallback<MouseLeaveEvent>(_  => _startBtnTargetX  = 0f);
        _creditBtn.RegisterCallback<MouseEnterEvent>(_ => _creditBtnTargetX = BtnHoverDist);
        _creditBtn.RegisterCallback<MouseLeaveEvent>(_ => _creditBtnTargetX = 0f);

        // レイアウト確定後に初期化
        _tankScene.RegisterCallback<GeometryChangedEvent>(_ => InitSceneLayout());
    }

    private void InitSceneLayout()
    {
        var r = _tankScene.contentRect;
        if (r.width <= 0) return;
        _sceneW = r.width;
        _sceneH = r.height;

        _playerX   = _sceneW * 0.18f;
        _playerCY  = _sceneH * 0.50f;
        _enemyBaseX = _sceneW * 0.72f;
        _enemyX     = _enemyBaseX;
        _enemyCY    = _sceneH * 0.50f;

        // 戦車を初期配置
        PlaceTank(_tankPlayer,        _playerX, _playerCY,  TankSize);
        PlaceTank(_tankTurretPlayer,  _playerX, _playerCY,  TurretSize);
        PlaceTank(_tankEnemy,         _enemyX,  _enemyCY,   TankSize);
        PlaceTank(_tankTurretEnemy,   _enemyX,  _enemyCY,   TurretSize);

        // プレイヤーは右向き、敵は左向き
        SetRotate(_tankPlayer,       90f);
        SetRotate(_tankTurretPlayer, 90f);
        SetRotate(_tankEnemy,       -90f);
        SetRotate(_tankTurretEnemy, -90f);
    }

    void Update()
    {
        if (_sceneW <= 0) return;
        float dt = Time.unscaledDeltaTime;

        // グリッドスクロール
        _gridOffsetX += GridSpeed * dt;
        _gridOffsetY += GridSpeed * dt;
        _gridBg.MarkDirtyRepaint();

        // ボタンホバー補間
        float smooth = 1f - Mathf.Exp(-14f * dt);
        _startBtnX  = Mathf.Lerp(_startBtnX,  _startBtnTargetX,  smooth);
        _creditBtnX = Mathf.Lerp(_creditBtnX, _creditBtnTargetX, smooth);
        SetTranslate(_startBtn,  _startBtnX,  0f);
        SetTranslate(_creditBtn, _creditBtnX, 0f);

        // ロゴ浮遊
        SetTranslate(_logoArea, 0f, Mathf.Sin(Time.unscaledTime * 0.9f) * 5f);

        // 敵パトロール
        UpdateEnemyPatrol(dt);

        // 弾
        UpdateBullet(dt);
    }

    private void UpdateEnemyPatrol(float dt)
    {
        _enemyX += _enemyDir * EnemySpeed * dt;
        if (_enemyX > _enemyBaseX + EnemyPatrolRange) { _enemyX = _enemyBaseX + EnemyPatrolRange; _enemyDir = -1f; }
        if (_enemyX < _enemyBaseX - EnemyPatrolRange) { _enemyX = _enemyBaseX - EnemyPatrolRange; _enemyDir =  1f; }

        float hitShake = 0f;
        if (_enemyHitTimer > 0f)
        {
            _enemyHitTimer -= Time.unscaledDeltaTime;
            hitShake = Mathf.Sin(_enemyHitTimer * 40f) * 7f;
        }

        PlaceTank(_tankEnemy,       _enemyX + hitShake, _enemyCY, TankSize);
        PlaceTank(_tankTurretEnemy, _enemyX + hitShake, _enemyCY, TurretSize);

        // 砲塔をプレイヤー方向に向ける
        float angle = Mathf.Atan2(_playerCY - _enemyCY, _playerX - _enemyX) * Mathf.Rad2Deg;
        SetRotate(_tankTurretEnemy, angle + 90f);
    }

    private void UpdateBullet(float dt)
    {
        if (!_bulletActive)
        {
            _fireCooldown -= dt;
            if (_fireCooldown <= 0f) FireBullet();
            return;
        }

        _bulletPos += _bulletVel * dt;

        // 上下で1回だけ反射
        if (!_bulletBounced && (_bulletPos.y < 15f || _bulletPos.y > _sceneH - 15f))
        {
            _bulletVel.y   = -_bulletVel.y;
            _bulletBounced = true;
        }

        _trailPoints.Add(_bulletPos);
        if (_trailPoints.Count > 25) _trailPoints.RemoveAt(0);
        _tankScene.MarkDirtyRepaint();

        // 敵にヒット
        if (Vector2.Distance(_bulletPos, new Vector2(_enemyX, _enemyCY)) < TankSize * 0.55f)
        {
            _enemyHitTimer = 0.45f;
            ResetBullet(cooldown: 3.5f);
            return;
        }

        // 画面外
        if (_bulletPos.x > _sceneW + 30f || _bulletPos.x < -30f)
        {
            ResetBullet(cooldown: 3f);
            return;
        }

        _bulletEl.style.left = new StyleLength(_bulletPos.x - BulletSize * 0.5f);
        _bulletEl.style.top  = new StyleLength(_bulletPos.y - BulletSize * 0.5f);
    }

    private void FireBullet()
    {
        _bulletActive  = true;
        _bulletBounced = false;
        _trailPoints.Clear();

        _bulletPos = new Vector2(_playerX + TankSize * 0.4f, _playerCY);

        // 発射方向：斜め上下にランダムに変化
        float angle = Random.value > 0.5f ? -30f : 30f;
        float rad   = angle * Mathf.Deg2Rad;
        _bulletVel  = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * 200f;

        _bulletEl.style.display = DisplayStyle.Flex;
    }

    private void ResetBullet(float cooldown)
    {
        _bulletActive           = false;
        _fireCooldown           = cooldown;
        _bulletEl.style.display = DisplayStyle.None;
        _trailPoints.Clear();
        _tankScene.MarkDirtyRepaint();
    }

    // ── 描画 ─────────────────────────────────

    private void DrawGrid(MeshGenerationContext ctx)
    {
        var p    = ctx.painter2D;
        var rect = _gridBg.contentRect;
        if (rect.width <= 0) return;

        // 明るいブルーグレー背景
        p.fillColor = new Color(0.847f, 0.894f, 0.941f, 1f);
        p.BeginPath();
        p.MoveTo(new Vector2(0,          0));
        p.LineTo(new Vector2(rect.width, 0));
        p.LineTo(new Vector2(rect.width, rect.height));
        p.LineTo(new Vector2(0,          rect.height));
        p.ClosePath();
        p.Fill();

        // 暗いネイビーグリッド線
        p.strokeColor = new Color(0.118f, 0.216f, 0.392f, 0.13f);
        p.lineWidth   = 1f;

        float ox = _gridOffsetX % GridCellSize;
        float oy = _gridOffsetY % GridCellSize;

        for (float x = -ox; x <= rect.width + GridCellSize; x += GridCellSize)
        {
            p.BeginPath();
            p.MoveTo(new Vector2(x, 0));
            p.LineTo(new Vector2(x, rect.height));
            p.Stroke();
        }
        for (float y = -oy; y <= rect.height + GridCellSize; y += GridCellSize)
        {
            p.BeginPath();
            p.MoveTo(new Vector2(0, y));
            p.LineTo(new Vector2(rect.width, y));
            p.Stroke();
        }
    }

    private void DrawTrail(MeshGenerationContext ctx)
    {
        if (_trailPoints.Count < 2) return;
        var p = ctx.painter2D;
        p.strokeColor = new Color(1f, 0.82f, 0.2f, 0.6f);
        p.lineWidth   = 2.5f;
        p.BeginPath();
        p.MoveTo(_trailPoints[0]);
        for (int i = 1; i < _trailPoints.Count; i++)
            p.LineTo(_trailPoints[i]);
        p.Stroke();
    }

    // ── ユーティリティ ────────────────────────

    private static void PlaceTank(VisualElement el, float cx, float cy, float size)
    {
        el.style.width  = new StyleLength(size);
        el.style.height = new StyleLength(size);
        el.style.left   = new StyleLength(cx - size * 0.5f);
        el.style.top    = new StyleLength(cy - size * 0.5f);
    }

    private static void SetTranslate(VisualElement el, float x, float y)
    {
        el.style.translate = new StyleTranslate(new Translate(
            new Length(x, LengthUnit.Pixel),
            new Length(y, LengthUnit.Pixel)));
    }

    private static void SetRotate(VisualElement el, float degrees)
    {
        el.style.rotate = new StyleRotate(new Rotate(new Angle(degrees, AngleUnit.Degree)));
    }

    private void OnStart()  => SceneManager.LoadScene("1_MainGame");
    private void OnCredit() { }
}
