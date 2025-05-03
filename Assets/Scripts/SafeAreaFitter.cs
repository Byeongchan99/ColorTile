using UnityEngine;

[Tooltip("모바일 기기의 Safe Area에 맞게 RectTransform을 조정")]
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    RectTransform _panel;

    void Awake()
    {
        _panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // 해상도나 기기 회전 등에 따라 Safe Area 값이 바뀌면 갱신
        if (_lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        _lastSafeArea = safeArea;

        // Safe Area의 픽셀 단위 값을 정규화(Normalized 0 ~ 1 사이의 값)로 변환
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // RectTransform의 Anchor를 Safe Area에 맞게 조정
        _panel.anchorMin = anchorMin;
        _panel.anchorMax = anchorMax;
        _panel.offsetMin = Vector2.zero;
        _panel.offsetMax = Vector2.zero;
    }
}
