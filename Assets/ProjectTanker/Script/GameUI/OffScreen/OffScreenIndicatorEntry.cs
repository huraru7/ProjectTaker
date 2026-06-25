using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicatorEntry : MonoBehaviour
{
    [SerializeField] private RectTransform    arrowRoot;
    [SerializeField] private Image            arrowImage;
    [SerializeField] private TextMeshProUGUI  distanceText;

    private RectTransform _rt;

    void Awake() => _rt = GetComponent<RectTransform>();

    public void Setup(Color color, string label = "")
    {
        if (arrowImage != null)
            arrowImage.color = color;
    }

    public void UpdateIndicator(Vector2 localPos, float angle, float distance)
    {
        _rt.localPosition = new Vector3(localPos.x, localPos.y, 0f);

        // arrowRoot を回転させて矢印をターゲット方向に向ける
        // デフォルトスプライトが上向きのため -90° オフセット
        if (arrowRoot != null)
            arrowRoot.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        if (distanceText != null)
            distanceText.text = $"{Mathf.RoundToInt(distance)}m";
    }
}
