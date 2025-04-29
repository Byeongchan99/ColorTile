using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static Enums; // Enums.cs에서 정의한 enum 사용

public class UIManager : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text resultText;
    public Slider timerSlider;
    public TMP_Text scoreText;

    public GameObject PauseUI;
    public GameObject MainUI;
    public GameObject ResultUI;

    [Header("Game Mode Settings")]
    public GameObject NormalModeArea;
    public GameObject InfiniteModeArea;

    public RectTransform panelRect;      // Panel의 RectTransform
    public LayoutElement topLayoutLE;    // Top Area의 LayoutElement
    public RectTransform normalRT;       // NormalModeArea의 RectTransform
    public RectTransform infiniteRT;     // InfiniteModeArea의 RectTransform

    public GameObject NormalModeGrid;
    public GameObject InfiniteModeGrid;

    [Header("References")]
    public PlayManager playManager;
    public GameManager gameManager;
    public StageGenerator stageGenerator;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        // 타이머 UI 업데이트 시 호출
        GameEvents.OnTimerUpdated += UpdateTimer;

        // 점수 UI 업데이트 시 호출
        GameEvents.OnScoreChanged += UpdateScore;

        // 게임 시작 시 호출
        GameEvents.OnGameStartedSecond += CloseMainUI; // 게임 시작 시 초기화
        GameEvents.OnGameStartedSecond += StartGame;
        GameEvents.OnGameStartedSecond += SwitchMode;

        // 게임 종료 시 호출
        GameEvents.OnGameEnded += EndGame;

        // 게임 재시작 시 호출
        GameEvents.OnRetryGame += StartGame;

        // 메인 화면으로 이동 시 호출
        GameEvents.OnGoToMainFirst += GoToMain;

        MainUI.SetActive(true); // 메인 UI 활성화
    }

    // 초기화
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% 남은 시간
        scoreText.text = "0";
    }

    // 타이머 UI 업데이트
    public void UpdateTimer(float value)
    {
        timerSlider.value = value;
    } 

    // 점수 UI 업데이트
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    // 노말 모드 게임 시작 버튼 클릭
    public void OnClickStartNormalButton()
    {
        GameManager.gameMode = GameMode.Normal; // 게임 모드 설정
        GameEvents.OnGameStartedRequest?.Invoke(); // 게임 시작 이벤트 호출
    }

    // 무한 모드 게임 시작 버튼 클릭
    public void OnClickStartInfiniteButton()
    {
        GameManager.gameMode = GameMode.Infinite; // 게임 모드 설정
        GameEvents.OnGameStartedRequest?.Invoke(); // 게임 시작 이벤트 호출
    }

    public void SwitchMode()
    {
        bool isNormal = GameManager.gameMode == GameMode.Normal;

        // 1) 두 모드를 한 번에 토글
        NormalModeArea.SetActive(isNormal);

        NormalModeGrid.SetActive(isNormal);
        InfiniteModeGrid.SetActive(!isNormal);

        // 2) 캔버스 레이아웃 최신화 (GetPreferredHeight 전)
        Canvas.ForceUpdateCanvases();

        // 3) 현재 활성화된 RectTransform으로 높이 계산
        RectTransform activeRT = isNormal ? normalRT : infiniteRT;
        float contentH = LayoutUtility.GetPreferredHeight(activeRT);

        // 4) Top Area 높이 할당 (min/preferred 동시)
        topLayoutLE.minHeight = contentH;
        topLayoutLE.preferredHeight = contentH;

        // 5) 패널 전체 레이아웃 재계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
    }

    public void OnClickRetryGameButton()
    {
        GameEvents.OnRetryGameRequest?.Invoke(); // 게임 재시작 이벤트 호출
    }

    // 게임 시작 처리
    public void StartGame()
    {
        //MainUI.SetActive(false);
        resultText.gameObject.SetActive(false);
        //ResultUI.SetActive(false);
        //playManager.Initialize();
        Initialize();
        //stageGenerator.GenerateStage();
    }

    // 일시정지 버튼 클릭
    public void OnClickPauseButton()
    {
        GameEvents.OnPauseGame?.Invoke(); // 게임 일시정지 이벤트 호출
        //gameManager.StopTime();
        PauseUI.SetActive(true); // 일시정지 UI 활성화
    }

    // 게임 종료 처리(win: true면 승리, false면 패배)
    public void EndGame(GameResult result)
    {
        //gameManager.StopTime();
        ShowResult(result);      
    }

    // 게임 종료 결과 UI 표시
    public void ShowResult(GameResult result)
    {
        ResultUI.SetActive(true);
        resultText.gameObject.SetActive(true);
        if (result == GameResult.Cleared)
        {
            resultText.text = "Stage cleared!\nYou win!\n\nScore: " + playManager.Score + "\nRemain Time: " + (int)playManager.timeRemaining;
        }
        else if (result == GameResult.NoRemovableTiles)
        {
            resultText.text = "No removable tiles!\nGame Over!\nScore: " + playManager.Score;
        }
        else if (result == GameResult.TimeOver)
        {
            resultText.text = "Time's up!\nGame Over!\nScore: " + playManager.Score;
        }
    } 

    // 메인 화면 닫기
    public void CloseMainUI()
    {
        MainUI.SetActive(false);
    }

    public void OnClickGoToMain()
    {
        GameEvents.OnGoToMainRequest?.Invoke(); // 메인 화면으로 이동 이벤트 호출
    }

    // 메인 화면으로 이동
    public void GoToMain()
    {
        MainUI.SetActive(true);
    }

    public void OnClickOpenOption()
    {
        GameEvents.OnOpenOption?.Invoke();
    }
}
