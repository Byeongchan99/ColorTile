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

    public GameObject TimerArea;
    public GameObject ScoreArea;

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
        GameEvents.OnGameStarted += StartGame;

        // 게임 종료 시 호출
        GameEvents.OnGameEnded += EndGame;

        // 게임 재시작 시 호출
        GameEvents.OnRetryGame += StartGame;

        // 메인 화면으로 이동 시 호출
        GameEvents.OnGoToMain += GoToMain;

        MainUI.SetActive(true); // 메인 UI 활성화
    }

    // 초기화
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% 남은 시간
        scoreText.text = "0";

        if (GameManager.gameMode == GameMode.Normal)
        {
            TimerArea.SetActive(true); // 타이머 UI 활성화
        }
        else if (GameManager.gameMode == GameMode.Infinite)
        {
            TimerArea.SetActive(false); // 타이머 UI 비활성화
        }
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

    // 게임 시작 버튼 클릭
    public void OnClickStartNormalButton()
    {
        GameManager.gameMode = GameMode.Normal; // 게임 모드 설정
        GameEvents.OnGameStarted?.Invoke(); // 게임 시작 이벤트 호출
        //gameManager.StartTime();
        MainUI.SetActive(false);
        StartGame();
    }

    public void OnClickStartInfiniteButton()
    {
        GameManager.gameMode = GameMode.Infinite; // 게임 모드 설정
        GameEvents.OnGameStarted?.Invoke(); // 게임 시작 이벤트 호출
        //gameManager.StartTime();
        MainUI.SetActive(false);
        StartGame();
    }

    public void OnClickRetryGameButton()
    {
        GameEvents.OnRetryGame?.Invoke(); // 게임 재시작 이벤트 호출
    }

    // 게임 시작 처리
    public void StartGame()
    {
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
        GoToMain();
    }

    // 메인 화면으로 이동
    public void GoToMain()
    {
        gameManager.StopTime();
        MainUI.SetActive(true);
    }

    public void OnClickOpenOption()
    {
        GameEvents.OnOpenOption?.Invoke();
    }
}
