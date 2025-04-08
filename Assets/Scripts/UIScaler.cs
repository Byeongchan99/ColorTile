using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ȭ���� ���� ������ A, B ������ ������,
/// B ����(Top Area)�� ���� ������ �ٽ� ������
/// Ÿ�̸�, ����, �Ͻ����� ��ư�� ��ġ�ϴ� ���� ��ũ��Ʈ
/// </summary>

public class UIScaler : MonoBehaviour
{
    [Header("A (Top) ����, B (Mid) ���� RectTransform")]
    public RectTransform topArea;    // B ����
    public RectTransform midArea; // A ����

    [Header("A (Top) ���� �� UI RectTransform")]
    public RectTransform timerUI;    // Ÿ�̸�
    public RectTransform scoreUI;    // ����
    public RectTransform pauseUI;    // �Ͻ����� ��ư

    [Header("���� ����(Top : Mid)")]
    [Tooltip("��) 2:8�̸� topRatio = 0.2, midRatio = 0.8")]
    [Range(0.0f, 1.0f)]
    public float topRatio = 0.05f;

    [Header("B (Top) ���� ���� ���� ����")]
    [Tooltip("�� �� �е� 5%, Ÿ�̸� 40%, ���� 35%, �Ͻ����� 15%, �е� 5%")]
    [Range(0f, 1f)] public float leftPaddingRatio = 0.05f;
    [Range(0f, 1f)] public float timerRatio = 0.40f;
    [Range(0f, 1f)] public float scoreRatio = 0.35f;
    [Range(0f, 1f)] public float pauseRatio = 0.15f;
    [Range(0f, 1f)] public float rightPaddingRatio = 0.05f;

    // ���� ȭ�� ũ��
    private float screenWidth;
    private float screenHeight;

    void Start()
    {
        // ���� �� �� �� UI ����
        UpdateUI();
    }

    /*
    void Update()
    {
        // ��� �ػ󵵳� â ũ�Ⱑ ����Ǵ� ��Ȳ�� �����Ϸ���
        // �� ������ Ȥ�� ���Ǻη� ������Ʈ���� ���� ����
        if (screenWidth != Screen.width || screenHeight != Screen.height)
        {
            UpdateUI();
        }
    }
    */

    private void UpdateUI()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // 1) ��ü ȭ���� ���η� A(��� ����), B(�ߴ� ����)���� ������
        // --------------------------------------------------------
        // - anchorMin, anchorMax�� �̿��� RectTransform�� ������ ��� ���
        // - Top Area�� ȭ�� ���ʿ� (0,1 - topRatio) ~ (1,1)
        // - Middle Area�� �� �Ʒ� (0,0) ~ (1,1 - topRatio)
        // --------------------------------------------------------

        // Top Area (A ����)
        topArea.anchorMin = new Vector2(0f, 1f - topRatio);
        topArea.anchorMax = new Vector2(1f, 1f);
        // offsetMin, offsetMax�� 0���� �����ؼ� ��Ŀ �������� �� ���߱�
        topArea.offsetMin = Vector2.zero;
        topArea.offsetMax = Vector2.zero;

        // Mid Area (B ����)
        midArea.anchorMin = new Vector2(0f, 0f);
        midArea.anchorMax = new Vector2(1f, 1f - topRatio);
        midArea.offsetMin = Vector2.zero;
        midArea.offsetMax = Vector2.zero;

        // 2) A(Top) ���� ���θ� ���� ������ ������
        // --------------------------------------------------------
        // - �е� 5% : Ÿ�̸� 40% : ���� 35% : �Ͻ����� 15% : �е� 5%
        // --------------------------------------------------------
        // RectTransform�� width, height�� UI�� ������ ��ġ�� �Ŀ��� ������
        // Start() ���������� ���� ���̾ƿ��� ���ŵ��� �ʾ��� ���� �����Ƿ�
        // �ʿ��ϴٸ� ForceUpdateRectTransforms() ���� ���� ���� ���� �� ���� ������ ���� ����
        // ���⼭�� �ܼ� ���÷� �ٷ� ���
        topArea.ForceUpdateRectTransforms();
        float topAreaWidth = topArea.rect.width;
        float topAreaHeight = topArea.rect.height;

        // ���� ��
        float totalRatio = leftPaddingRatio + timerRatio + scoreRatio + pauseRatio + rightPaddingRatio;

        // ���� �� ��Ұ� ������ width
        float leftPaddingWidth = topAreaWidth * (leftPaddingRatio / totalRatio);
        float timerWidth = topAreaWidth * (timerRatio / totalRatio);
        float scoreWidth = topAreaWidth * (scoreRatio / totalRatio);
        float pauseWidth = topAreaWidth * (pauseRatio / totalRatio);
        // rightPaddingWidth�� ���� UI ��Ұ� �ƴϹǷ� ��븸 ���
        float rightPaddingWidth = topAreaWidth * (rightPaddingRatio / totalRatio);

        // x ��ġ�� �����ϸ鼭 �� ��� ��ġ
        float currentX = 0f;

        // 2-1) Left Padding(����) ��ŵ
        currentX += leftPaddingWidth;

        // 2-2) Ÿ�̸�
        // anchorMin, anchorMax�� (0, 0) ~ (0, 1)�� ������ �θ�(RectTransform)�� ���� �������� ���� ���� ����
        timerUI.anchorMin = new Vector2(0f, 0f);
        timerUI.anchorMax = new Vector2(0f, 1f);
        timerUI.pivot = new Vector2(0f, 0.5f); // ���� ����� Pivot����
        timerUI.anchoredPosition = new Vector2(currentX, 0f);
        timerUI.sizeDelta = new Vector2(timerWidth, topAreaHeight);
        currentX += timerWidth;

        // 2-3) ����
        scoreUI.anchorMin = new Vector2(0f, 0f);
        scoreUI.anchorMax = new Vector2(0f, 1f);
        scoreUI.pivot = new Vector2(0f, 0.5f);
        scoreUI.anchoredPosition = new Vector2(currentX, 0f);
        scoreUI.sizeDelta = new Vector2(scoreWidth, topAreaHeight);
        currentX += scoreWidth;

        // 2-4) �Ͻ����� ��ư
        pauseUI.anchorMin = new Vector2(0f, 0f);
        pauseUI.anchorMax = new Vector2(0f, 1f);
        pauseUI.pivot = new Vector2(0f, 0.5f);
        pauseUI.anchoredPosition = new Vector2(currentX, 0f);
        pauseUI.sizeDelta = new Vector2(pauseWidth, topAreaHeight);
        currentX += pauseWidth;

        // 2-5) Right Padding(����) ��ŵ
        currentX += rightPaddingWidth;

        // 3) A(Middle) ���� ������ ������ ũ�� ����(����)
        // --------------------------------------------------------
        // - �������� ���� ������Ʈ(��������Ʈ�� 2D ������Ʈ)�� �� �����Ƿ�,
        //   RectTransform�� �ƴ� Transform(Ȥ�� SpriteRenderer ��)�� ������ ������ ���� ����
        // - �Ʒ��� Middle Area ũ�⿡ ���� �������� ũ�⸦ ���ߴ� ������ ����
        // --------------------------------------------------------
        // middleArea.ForceUpdateRectTransforms();
        // float middleAreaWidth = middleArea.rect.width;
        // float middleAreaHeight = middleArea.rect.height;
        // Vector2 boardSize = new Vector2(middleAreaWidth, middleAreaHeight);
        // boardObject.transform.localScale = CalculateBoardScale(boardObject, boardSize);

        // �ʿ��ϴٸ� ���� ���� ������� ������ ������ ���� ������ �߰�
    }

    // ����) ������ ũ�⸦ ���ߴ� �޼���(���� ���� �ʿ�)
    // private Vector3 CalculateBoardScale(GameObject board, Vector2 targetSize)
    // {
    //     // ���� ������Ʈ�� ���� ũ��(����, ����)�� ���� targetSize�� �°� ���� ����
    //     // ...
    // }
}
