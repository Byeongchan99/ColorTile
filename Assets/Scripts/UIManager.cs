using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static Enums; // Enums.cs���� ������ enum ���

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
        // Ÿ�̸� UI ������Ʈ �� ȣ��
        GameEvents.OnTimerUpdated += UpdateTimer;

        // ���� UI ������Ʈ �� ȣ��
        GameEvents.OnScoreChanged += UpdateScore;

        // ���� ���� �� ȣ��
        GameEvents.OnGameStarted += StartGame;

        // ���� ���� �� ȣ��
        GameEvents.OnGameEnded += EndGame;

        // ���� ����� �� ȣ��
        GameEvents.OnRetryGame += StartGame;

        // ���� ȭ������ �̵� �� ȣ��
        GameEvents.OnGoToMain += GoToMain;

        MainUI.SetActive(true); // ���� UI Ȱ��ȭ
    }

    // �ʱ�ȭ
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% ���� �ð�
        scoreText.text = "0";

        if (GameManager.gameMode == GameMode.Normal)
        {
            TimerArea.SetActive(true); // Ÿ�̸� UI Ȱ��ȭ
        }
        else if (GameManager.gameMode == GameMode.Infinite)
        {
            TimerArea.SetActive(false); // Ÿ�̸� UI ��Ȱ��ȭ
        }
    }

    // Ÿ�̸� UI ������Ʈ
    public void UpdateTimer(float value)
    {
        timerSlider.value = value;
    } 

    // ���� UI ������Ʈ
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    // ���� ���� ��ư Ŭ��
    public void OnClickStartNormalButton()
    {
        GameManager.gameMode = GameMode.Normal; // ���� ��� ����
        GameEvents.OnGameStarted?.Invoke(); // ���� ���� �̺�Ʈ ȣ��
        //gameManager.StartTime();
        MainUI.SetActive(false);
        StartGame();
    }

    public void OnClickStartInfiniteButton()
    {
        GameManager.gameMode = GameMode.Infinite; // ���� ��� ����
        GameEvents.OnGameStarted?.Invoke(); // ���� ���� �̺�Ʈ ȣ��
        //gameManager.StartTime();
        MainUI.SetActive(false);
        StartGame();
    }

    public void OnClickRetryGameButton()
    {
        GameEvents.OnRetryGame?.Invoke(); // ���� ����� �̺�Ʈ ȣ��
    }

    // ���� ���� ó��
    public void StartGame()
    {
        resultText.gameObject.SetActive(false);
        //ResultUI.SetActive(false);
        //playManager.Initialize();
        Initialize();
        //stageGenerator.GenerateStage();
    }

    // �Ͻ����� ��ư Ŭ��
    public void OnClickPauseButton()
    {
        GameEvents.OnPauseGame?.Invoke(); // ���� �Ͻ����� �̺�Ʈ ȣ��
        //gameManager.StopTime();
        PauseUI.SetActive(true); // �Ͻ����� UI Ȱ��ȭ
    }

    // ���� ���� ó��(win: true�� �¸�, false�� �й�)
    public void EndGame(GameResult result)
    {
        //gameManager.StopTime();
        ShowResult(result);      
    }

    // ���� ���� ��� UI ǥ��
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

    // ���� ȭ�� �ݱ�
    public void CloseMainUI()
    {
        MainUI.SetActive(false);
    }

    public void OnClickGoToMain()
    {
        GoToMain();
    }

    // ���� ȭ������ �̵�
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
