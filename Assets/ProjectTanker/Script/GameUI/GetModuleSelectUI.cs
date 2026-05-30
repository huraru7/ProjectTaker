using R3;
using UnityEngine;

public class GetModuleSelectUI : MonoBehaviour
{
    //:モジュール獲得のUI　Presenterから3択を受け取って表示し、選んだモジュールをPresenterに通知する
    [SerializeField] private GameObject panel;
    [SerializeField] private ModuleOptionButton[] optionButtons;

    private readonly Subject<ModuleData> _onModuleSelected = new();
    public Observable<ModuleData> OnModuleSelected => _onModuleSelected;

    /// <summary>
    /// 3択UIを表示する　Presenterから呼ばれる
    /// </summary>
    public void ShowOptions(ModuleData[] candidates)
    {
        panel.SetActive(true);
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < candidates.Length)
            {
                ModuleData candidate = candidates[i];
                optionButtons[i].Setup(candidate, () => OnOptionChosen(candidate));
                optionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnOptionChosen(ModuleData chosen)
    {
        panel.SetActive(false);
        _onModuleSelected.OnNext(chosen);
    }

    private void OnDestroy() => _onModuleSelected.Dispose();
}
