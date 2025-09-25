using TMPro;
using UnityEngine;

public static class TextUI
{
    [SerializeField] private const string AssetPath = "Text";
    public static void ShowText(string text, Vector2 screenPosition, float duration = 1f, float size = 36f)
    {
        TextMeshProUGUI instance = UnityEngine.Object.Instantiate(Resources.Load<TextMeshProUGUI>(AssetPath));
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        instance.transform.SetParent(canvas.transform);
        instance.fontSize = size;
        instance.text = text;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out var localPoint);
        instance.rectTransform.anchoredPosition = localPoint;
        UnityEngine.Object.Destroy(instance.gameObject, duration);
    }
}
