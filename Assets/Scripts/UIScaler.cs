using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면을 세로 비율로 A, B 영역을 나누고,
/// B 영역(Top Area)을 가로 비율로 다시 나누어
/// 타이머, 점수, 일시정지 버튼을 배치하는 예시 스크립트
/// </summary>

public class UIScaler : MonoBehaviour
{
    [Header("A (Top) 영역, B (Mid) 영역 RectTransform")]
    public RectTransform topArea;    // B 영역
    public RectTransform midArea; // A 영역

    [Header("A (Top) 영역 내 UI RectTransform")]
    public RectTransform timerUI;    // 타이머
    public RectTransform scoreUI;    // 점수
    public RectTransform pauseUI;    // 일시정지 버튼

    [Header("세로 비율(Top : Mid)")]
    [Tooltip("예) 2:8이면 topRatio = 0.2, midRatio = 0.8")]
    [Range(0.0f, 1.0f)]
    public float topRatio = 0.05f;

    [Header("B (Top) 영역 내부 가로 비율")]
    [Tooltip("좌 → 패딩 5%, 타이머 40%, 점수 35%, 일시정지 15%, 패딩 5%")]
    [Range(0f, 1f)] public float leftPaddingRatio = 0.05f;
    [Range(0f, 1f)] public float timerRatio = 0.40f;
    [Range(0f, 1f)] public float scoreRatio = 0.35f;
    [Range(0f, 1f)] public float pauseRatio = 0.15f;
    [Range(0f, 1f)] public float rightPaddingRatio = 0.05f;

    // 실제 화면 크기
    private float screenWidth;
    private float screenHeight;

    void Start()
    {
        // 시작 시 한 번 UI 세팅
        UpdateUI();
    }

    /*
    void Update()
    {
        // 기기 해상도나 창 크기가 변경되는 상황에 대응하려면
        // 매 프레임 혹은 조건부로 업데이트해줄 수도 있음
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

        // 1) 전체 화면을 세로로 A(상단 영역), B(중단 영역)으로 나누기
        // --------------------------------------------------------
        // - anchorMin, anchorMax를 이용해 RectTransform의 영역을 잡는 방식
        // - Top Area는 화면 위쪽에 (0,1 - topRatio) ~ (1,1)
        // - Middle Area는 그 아래 (0,0) ~ (1,1 - topRatio)
        // --------------------------------------------------------

        // Top Area (A 영역)
        topArea.anchorMin = new Vector2(0f, 1f - topRatio);
        topArea.anchorMax = new Vector2(1f, 1f);
        // offsetMin, offsetMax를 0으로 설정해서 앵커 기준으로 딱 맞추기
        topArea.offsetMin = Vector2.zero;
        topArea.offsetMax = Vector2.zero;

        // Mid Area (B 영역)
        midArea.anchorMin = new Vector2(0f, 0f);
        midArea.anchorMax = new Vector2(1f, 1f - topRatio);
        midArea.offsetMin = Vector2.zero;
        midArea.offsetMax = Vector2.zero;

        // 2) A(Top) 영역 내부를 가로 비율로 나누기
        // --------------------------------------------------------
        // - 패딩 5% : 타이머 40% : 점수 35% : 일시정지 15% : 패딩 5%
        // --------------------------------------------------------
        // RectTransform의 width, height는 UI가 실제로 배치된 후에야 결정됨
        // Start() 시점에서는 아직 레이아웃이 갱신되지 않았을 수도 있으므로
        // 필요하다면 ForceUpdateRectTransforms() 등을 통해 강제 갱신 후 값을 가져올 수도 있음
        // 여기서는 단순 예시로 바로 사용
        topArea.ForceUpdateRectTransforms();
        float topAreaWidth = topArea.rect.width;
        float topAreaHeight = topArea.rect.height;

        // 비율 합
        float totalRatio = leftPaddingRatio + timerRatio + scoreRatio + pauseRatio + rightPaddingRatio;

        // 실제 각 요소가 차지할 width
        float leftPaddingWidth = topAreaWidth * (leftPaddingRatio / totalRatio);
        float timerWidth = topAreaWidth * (timerRatio / totalRatio);
        float scoreWidth = topAreaWidth * (scoreRatio / totalRatio);
        float pauseWidth = topAreaWidth * (pauseRatio / totalRatio);
        // rightPaddingWidth는 굳이 UI 요소가 아니므로 사용만 계산
        float rightPaddingWidth = topAreaWidth * (rightPaddingRatio / totalRatio);

        // x 위치를 누적하면서 각 요소 배치
        float currentX = 0f;

        // 2-1) Left Padding(공간) 스킵
        currentX += leftPaddingWidth;

        // 2-2) 타이머
        // anchorMin, anchorMax를 (0, 0) ~ (0, 1)로 잡으면 부모(RectTransform)의 왼쪽 기준으로 폭을 설정 가능
        timerUI.anchorMin = new Vector2(0f, 0f);
        timerUI.anchorMax = new Vector2(0f, 1f);
        timerUI.pivot = new Vector2(0f, 0.5f); // 왼쪽 가운데를 Pivot으로
        timerUI.anchoredPosition = new Vector2(currentX, 0f);
        timerUI.sizeDelta = new Vector2(timerWidth, topAreaHeight);
        currentX += timerWidth;

        // 2-3) 점수
        scoreUI.anchorMin = new Vector2(0f, 0f);
        scoreUI.anchorMax = new Vector2(0f, 1f);
        scoreUI.pivot = new Vector2(0f, 0.5f);
        scoreUI.anchoredPosition = new Vector2(currentX, 0f);
        scoreUI.sizeDelta = new Vector2(scoreWidth, topAreaHeight);
        currentX += scoreWidth;

        // 2-4) 일시정지 버튼
        pauseUI.anchorMin = new Vector2(0f, 0f);
        pauseUI.anchorMax = new Vector2(0f, 1f);
        pauseUI.pivot = new Vector2(0f, 0.5f);
        pauseUI.anchoredPosition = new Vector2(currentX, 0f);
        pauseUI.sizeDelta = new Vector2(pauseWidth, topAreaHeight);
        currentX += pauseWidth;

        // 2-5) Right Padding(공간) 스킵
        currentX += rightPaddingWidth;

        // 3) A(Middle) 영역 내부의 보드판 크기 조절(예시)
        // --------------------------------------------------------
        // - 보드판은 게임 오브젝트(스프라이트나 2D 오브젝트)일 수 있으므로,
        //   RectTransform이 아닌 Transform(혹은 SpriteRenderer 등)을 스케일 조절할 수도 있음
        // - 아래는 Middle Area 크기에 맞춰 보드판의 크기를 맞추는 간단한 예시
        // --------------------------------------------------------
        // middleArea.ForceUpdateRectTransforms();
        // float middleAreaWidth = middleArea.rect.width;
        // float middleAreaHeight = middleArea.rect.height;
        // Vector2 boardSize = new Vector2(middleAreaWidth, middleAreaHeight);
        // boardObject.transform.localScale = CalculateBoardScale(boardObject, boardSize);

        // 필요하다면 위와 같은 방식으로 보드판 스케일 조정 로직을 추가
    }

    // 예시) 보드판 크기를 맞추는 메서드(직접 구현 필요)
    // private Vector3 CalculateBoardScale(GameObject board, Vector2 targetSize)
    // {
    //     // 보드 오브젝트의 원본 크기(가로, 세로)를 구해 targetSize에 맞게 비율 조정
    //     // ...
    // }
}
