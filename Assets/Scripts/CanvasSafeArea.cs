using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasSafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform safeAreaRect;
    private Canvas _canvas;

    private Rect _lastSafeArea = Rect.zero;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        _lastSafeArea = Screen.safeArea;
        ApplySafeArea();
    }

    private void Update()
    {
        if (_lastSafeArea != Screen.safeArea)
        {
            _lastSafeArea = Screen.safeArea;
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (safeAreaRect == null) return;

        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        var pixelRect = _canvas.pixelRect;
        anchorMin.x /= pixelRect.width;
        anchorMin.y /= pixelRect.height;
        anchorMax.x /= pixelRect.width;
        anchorMax.y /= pixelRect.height;

        safeAreaRect.anchorMin = anchorMin;
        safeAreaRect.anchorMax = anchorMax;
    }
}