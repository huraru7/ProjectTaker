using R3;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleReplaceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image newModuleIcon;
    [SerializeField] private TextMeshProUGUI newModuleName;
    [SerializeField] private ModuleOptionButton[] slotOptionButtons;
    [SerializeField] private Button discardButton;
    [SerializeField] private float staggerDelay = 0.08f;

    // -1 = 破棄、0〜6 = 入れ替えるスロット番号
    private readonly Subject<int> _onDecision = new();
    public Observable<int> OnDecision => _onDecision;

    public void Show(ModuleData newModule, IReadOnlyList<ModuleData> currentSlots)
    {
        panel.SetActive(true);
        newModuleIcon.sprite = newModule.icon;
        newModuleName.text = newModule.moduleName;

        for (int i = 0; i < slotOptionButtons.Length; i++)
        {
            int capturedIndex = i;
            ModuleData slotModule = i < currentSlots.Count ? currentSlots[i] : null;
            slotOptionButtons[i].gameObject.SetActive(slotModule != null);
            if (slotModule != null)
            {
                slotOptionButtons[i].Setup(slotModule, () => Decide(capturedIndex));
                slotOptionButtons[i].PlayIntro(i * staggerDelay);
            }
        }

        discardButton.onClick.RemoveAllListeners();
        discardButton.onClick.AddListener(() => Decide(-1));
    }

    private void Decide(int slotIndex)
    {
        panel.SetActive(false);
        _onDecision.OnNext(slotIndex);
    }

    private void OnDestroy() => _onDecision.Dispose();
}
