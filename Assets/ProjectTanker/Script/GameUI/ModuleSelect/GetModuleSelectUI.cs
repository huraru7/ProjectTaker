using LitMotion;
using R3;
using UnityEngine;

public class GetModuleSelectUI : MonoBehaviour
{
    //:モジュール獲得のUI　Presenterから3択を受け取って表示し、選んだモジュールをPresenterに通知する
    [SerializeField] private GameObject panel;
    [SerializeField] private ModuleOptionButton[] optionButtons;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private float staggerDelay = 0.08f;

    private readonly Subject<ModuleData> _onModuleSelected = new();
    public Observable<ModuleData> OnModuleSelected => _onModuleSelected;

    /// <summary>
    /// 3択UIを表示する　Presenterから呼ばれる
    /// </summary>
    public void ShowOptions(ModuleData[] candidates)
    {
        panel.SetActive(true);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            LMotion.Create(0f, 1f, 0.2f)
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

    private void OnDestroy() => _onModuleSelected.Dispose();
}
