using UnityEngine;

[Tooltip("����� ����� Safe Area�� �°� RectTransform�� ����")]
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
        // �ػ󵵳� ��� ȸ�� � ���� Safe Area ���� �ٲ�� ����
        if (_lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        _lastSafeArea = safeArea;

        // Safe Area�� �ȼ� ���� ���� ����ȭ(Normalized 0 ~ 1 ������ ��)�� ��ȯ
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // RectTransform�� Anchor�� Safe Area�� �°� ����
        _panel.anchorMin = anchorMin;
        _panel.anchorMax = anchorMax;
        _panel.offsetMin = Vector2.zero;
        _panel.offsetMax = Vector2.zero;
    }
}
