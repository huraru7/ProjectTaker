using LitMotion;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GetModuleSelectUI : MonoBehaviour
{
    //:モジュール獲得のUI　Presenterから3択を受け取って表示し、選んだモジュールをPresenterに通知する
    [SerializeField] private GameObject panel;
    [SerializeField] private ModuleOptionButton[] optionButtons;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private float staggerDelay = 0.08f;

    [Header("リロール")]
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI rerollCountText;

    private MotionHandle _panelFadeHandle;

    private readonly Subject<ModuleData> _onModuleSelected = new();
    public Observable<ModuleData> OnModuleSelected => _onModuleSelected;

    private readonly Subject<Unit> _onRerollRequested = new();
    public Observable<Unit> OnRerollRequested => _onRerollRequested;

    void Awake()
    {
        if (rerollButton != null)
            rerollButton.onClick.AddListener(() => _onRerollRequested.OnNext(Unit.Default));
    }

    void Update()
    {
        if (panel.activeSelf && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            _onRerollRequested.OnNext(Unit.Default);
    }

    /// <summary>リロール残数の表示を更新する　Presenterから呼ばれる</summary>
    public void UpdateRerollCount(int count)
    {
        if (rerollCountText != null) rerollCountText.text = $"リロール ({count})";
        if (rerollButton != null) rerollButton.interactable = count > 0;
    }

    /// <summary>
    /// 3択UIを表示する　Presenterから呼ばれる
    /// </summary>
    public void ShowOptions(ModuleData[] candidates)
    {
        AudioManager.Instance?.PlayModuleOpen();
        panel.SetActive(true);

        if (panelCanvasGroup != null)
        {
            if (_panelFadeHandle.IsActive()) _panelFadeHandle.Cancel();
            panelCanvasGroup.alpha = 0f;
            _panelFadeHandle = LMotion.Create(0f, 1f, 0.2f)
                .Bind(a => panelCanvasGroup.alpha = a)
                .AddTo(this);
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < candidates.Length)
            {
                ModuleData candidate = candidates[i];
                ModuleOptionButton btn = optionButtons[i];
                btn.Setup(candidate, () => OnOptionChosen(candidate, btn));
                btn.gameObject.SetActive(true);
                btn.PlayIntro(i * staggerDelay);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnOptionChosen(ModuleData chosen, ModuleOptionButton chosenButton)
    {
        foreach (var btn in optionButtons)
        {
            if (!btn.gameObject.activeSelf) continue;
            if (btn == chosenButton) btn.PlaySelected(() => ClosePanel(chosen));
            else btn.PlayDismiss();
        }
    }

    private void ClosePanel(ModuleData chosen)
    {
        panel.SetActive(false);
        _onModuleSelected.OnNext(chosen);
    }

    private void OnDestroy()
    {
        _onModuleSelected.Dispose();
        _onRerollRequested.Dispose();
    }
}
