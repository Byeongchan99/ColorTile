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

    // TextMeshPro에서 색상 태그
    string colorTagStart = "<color=#905734>";
    string colorTagEnd = "</color>";

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

        if (!PlayerPrefs.HasKey("NormalHighScore"))
        {
            PlayerPrefs.SetInt("NormalHighScore", 0);
        }

        if (!PlayerPrefs.HasKey("NormalBestTime"))
        {
            PlayerPrefs.SetInt("NormalBestTime", 9999);
        }

        if (!PlayerPrefs.HasKey("InfiniteHighScore"))
        {
            PlayerPrefs.SetInt("InfiniteHighScore", 0);
        }
    }

    private void OnEnable()
    {
        // 타이머 UI 업데이트 시 호출
        GameEvents.OnTimerUpdated += UpdateTimer;

        // 점수 UI 업데이트 시 호출
        GameEvents.OnScoreChanged += UpdateScore;

        // 게임 시작 시 호출
        GameEvents.OnGameStarted += CloseMainUI; // 게임 시작 시 초기화
        GameEvents.OnGameStarted += StartGame;
        GameEvents.OnGameStarted += SwitchMode;

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
        scoreText.text = colorTagStart + score.ToString() + colorTagEnd;
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
        if (GameManager.gameMode == GameMode.Normal)
            ShowNormalResult(result);
        else if (GameManager.gameMode == GameMode.Infinite)
            ShowInfiniteResult(result);  
    }

    // 게임 종료 결과 UI 표시
    public void ShowNormalResult(GameResult result)
    {
        // 1) 현재 세션 점수
        int currentScore = playManager.Score;

        // 2) 저장된 최고 점수 불러오기
        int bestScore = PlayerPrefs.GetInt("NormalHighScore", 0);
        //int bestTime = PlayerPrefs.GetInt("NormalBestTime", 0);

        // 3) 최고 점수 갱신
        if (currentScore > bestScore)
        {
            bestScore = currentScore;

            PlayerPrefs.SetInt("NormalHighScore", bestScore);
            //PlayerPrefs.SetInt("NormalBestTime", (int)playManager.timeRemaining);
            
            PlayerPrefs.Save(); 
        }
        else if (currentScore == bestScore) // 최고 점수와 동일하지만 시간 기록 갱신
        {
            bestScore = currentScore;

            PlayerPrefs.SetInt("NormalHighScore", bestScore);
            /*
            if (playManager.timeRemaining > bestTime)
            {
                bestTime = (int)playManager.timeRemaining;
                PlayerPrefs.SetInt("NormalBestTime", (int)playManager.timeRemaining);
            }
            */
            PlayerPrefs.Save();
        }

        // 4) UI 띄우기
        ResultUI.SetActive(true);
        resultText.gameObject.SetActive(true);

        // 공통으로 항상 보여줄 내용 (현재 점수, 최고 기록)
        //string scoreInfo = $"\n\nYour Score: {currentScore}\nBest Score: {bestScore}\nBest Remain Time: {bestTime}";
        string scoreInfo = $"\n\nYour Score: {currentScore}\nBest Score: {bestScore}";

        if (result == GameResult.Cleared)
        {
            //resultText.text = colorTagStart + "Stage cleared!\nYou win!" + scoreInfo + $"\nRemain Time: {(int)playManager.timeRemaining}" + colorTagEnd;
            resultText.text = colorTagStart + "Stage cleared!\nYou win!" + scoreInfo + colorTagEnd;
        }
        else if (result == GameResult.NoRemovableTiles)
        {
            resultText.text = colorTagStart + "No removable tiles!\nGame Over!" + scoreInfo + colorTagEnd;
        }
        else if (result == GameResult.TimeOver)
        {
            resultText.text = colorTagStart + "Time's up!\nGame Over!" + scoreInfo + colorTagEnd;
        }
    }

    public void ShowInfiniteResult(GameResult result)
    {
        // 1) 현재 세션 점수
        int currentScore = playManager.Score;

        // 2) 저장된 최고 점수 불러오기
        int bestScore = PlayerPrefs.GetInt("InfiniteHighScore", 0);

        // 3) 최고 점수 갱신
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("InfiniteHighScore", bestScore);
            PlayerPrefs.Save();  // 즉시 디스크에 저장
        }

        // 4) UI 띄우기
        ResultUI.SetActive(true);
        resultText.gameObject.SetActive(true);

        // 공통으로 항상 보여줄 내용 (현재 점수, 최고 기록)
        string scoreInfo = $"\n\nYour Score: {currentScore}\nBest Score: {bestScore}";

        if (result == GameResult.NoRemovableTiles)
        {
            resultText.text = colorTagStart + "No removable tiles!\nGame Over!" + scoreInfo + colorTagEnd;
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
