using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ProjectTanker.UI;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image elementDot;  // 属性を示す小さな丸
    [SerializeField] private ThemeColor theme;

    public ModuleData Data { get; private set; }
    public static ModuleData DraggingData { get; private set; }

    private GameObject _ghost;
    private Canvas _rootCanvas;

    public void Setup(ModuleData data)
    {
        Data = data;
        bool isEmpty = data == null;
        iconImage.sprite = isEmpty ? null : data.icon;
        nameText.text = isEmpty ? "" : data.moduleName;

        if (elementDot != null && theme != null)
        {
            elementDot.gameObject.SetActive(!isEmpty);
            if (!isEmpty)
                elementDot.color = GetElementColor(data.moduleElement);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Data == null) return;
        DraggingData = Data;
        _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

        _ghost = new GameObject("DragGhost");
        _ghost.transform.SetParent(_rootCanvas.transform, false);
        var img = _ghost.AddComponent<Image>();
        img.sprite = iconImage.sprite;
        img.raycastTarget = false;
        img.color = new Color(1f, 1f, 1f, 0.7f);
        _ghost.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData) { if (_ghost == null) return; UpdateGhostPosition(eventData); }

    public void OnEndDrag(PointerEventData eventData)
    {
        DraggingData = null;
        if (_ghost != null) Destroy(_ghost);
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.GetComponent<RectTransform>(),
            eventData.position,
            _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out var localPos);
        _ghost.GetComponent<RectTransform>().localPosition = localPos;
    }

    private Color GetElementColor(ModuleElementEnum element) => element switch
    {
        ModuleElementEnum.Earth => theme.earth,
        ModuleElementEnum.Water => theme.water,
        ModuleElementEnum.Fire  => theme.fire,
        ModuleElementEnum.Wind  => theme.wind,
        _                       => theme.none,
    };
}
