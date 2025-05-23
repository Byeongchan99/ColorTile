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

    // TextMeshPro���� ���� �±�
    string colorTagStart = "<color=#905734>";
    string colorTagEnd = "</color>";

    [Header("Game Mode Settings")]
    public GameObject NormalModeArea;
    public GameObject InfiniteModeArea;

    public RectTransform panelRect;      // Panel�� RectTransform
    public LayoutElement topLayoutLE;    // Top Area�� LayoutElement
    public RectTransform normalRT;       // NormalModeArea�� RectTransform
    public RectTransform infiniteRT;     // InfiniteModeArea�� RectTransform

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
        // Ÿ�̸� UI ������Ʈ �� ȣ��
        GameEvents.OnTimerUpdated += UpdateTimer;

        // ���� UI ������Ʈ �� ȣ��
        GameEvents.OnScoreChanged += UpdateScore;

        // ���� ���� �� ȣ��
        GameEvents.OnGameStarted += CloseMainUI; // ���� ���� �� �ʱ�ȭ
        GameEvents.OnGameStarted += StartGame;
        GameEvents.OnGameStarted += SwitchMode;

        // ���� ���� �� ȣ��
        GameEvents.OnGameEnded += EndGame;

        // ���� ����� �� ȣ��
        GameEvents.OnRetryGame += StartGame;

        // ���� ȭ������ �̵� �� ȣ��
        GameEvents.OnGoToMainFirst += GoToMain;

        MainUI.SetActive(true); // ���� UI Ȱ��ȭ
    }

    // �ʱ�ȭ
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% ���� �ð�
        scoreText.text = "0";
    }

    // Ÿ�̸� UI ������Ʈ
    public void UpdateTimer(float value)
    {
        timerSlider.value = value;
    } 

    // ���� UI ������Ʈ
    public void UpdateScore(int score)
    {
        scoreText.text = colorTagStart + score.ToString() + colorTagEnd;
    }

    // �븻 ��� ���� ���� ��ư Ŭ��
    public void OnClickStartNormalButton()
    {
        GameManager.gameMode = GameMode.Normal; // ���� ��� ����
        GameEvents.OnGameStartedRequest?.Invoke(); // ���� ���� �̺�Ʈ ȣ��
    }

    // ���� ��� ���� ���� ��ư Ŭ��
    public void OnClickStartInfiniteButton()
    {
        GameManager.gameMode = GameMode.Infinite; // ���� ��� ����
        GameEvents.OnGameStartedRequest?.Invoke(); // ���� ���� �̺�Ʈ ȣ��
    }

    public void SwitchMode()
    {
        bool isNormal = GameManager.gameMode == GameMode.Normal;

        // 1) �� ��带 �� ���� ���
        NormalModeArea.SetActive(isNormal);

        NormalModeGrid.SetActive(isNormal);
        InfiniteModeGrid.SetActive(!isNormal);

        // 2) ĵ���� ���̾ƿ� �ֽ�ȭ (GetPreferredHeight ��)
        Canvas.ForceUpdateCanvases();

        // 3) ���� Ȱ��ȭ�� RectTransform���� ���� ���
        RectTransform activeRT = isNormal ? normalRT : infiniteRT;
        float contentH = LayoutUtility.GetPreferredHeight(activeRT);

        // 4) Top Area ���� �Ҵ� (min/preferred ����)
        topLayoutLE.minHeight = contentH;
        topLayoutLE.preferredHeight = contentH;

        // 5) �г� ��ü ���̾ƿ� ����
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
    }

    public void OnClickRetryGameButton()
    {
        GameEvents.OnRetryGameRequest?.Invoke(); // ���� ����� �̺�Ʈ ȣ��
    }

    // ���� ���� ó��
    public void StartGame()
    {
        //MainUI.SetActive(false);
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
        if (GameManager.gameMode == GameMode.Normal)
            ShowNormalResult(result);
        else if (GameManager.gameMode == GameMode.Infinite)
            ShowInfiniteResult(result);  
    }

    // ���� ���� ��� UI ǥ��
    public void ShowNormalResult(GameResult result)
    {
        // 1) ���� ���� ����
        int currentScore = playManager.Score;

        // 2) ����� �ְ� ���� �ҷ�����
        int bestScore = PlayerPrefs.GetInt("NormalHighScore", 0);
        //int bestTime = PlayerPrefs.GetInt("NormalBestTime", 0);

        // 3) �ְ� ���� ����
        if (currentScore > bestScore)
        {
            bestScore = currentScore;

            PlayerPrefs.SetInt("NormalHighScore", bestScore);
            //PlayerPrefs.SetInt("NormalBestTime", (int)playManager.timeRemaining);
            
            PlayerPrefs.Save(); 
        }
        else if (currentScore == bestScore) // �ְ� ������ ���������� �ð� ��� ����
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

        // 4) UI ����
        ResultUI.SetActive(true);
        resultText.gameObject.SetActive(true);

        // �������� �׻� ������ ���� (���� ����, �ְ� ���)
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
        // 1) ���� ���� ����
        int currentScore = playManager.Score;

        // 2) ����� �ְ� ���� �ҷ�����
        int bestScore = PlayerPrefs.GetInt("InfiniteHighScore", 0);

        // 3) �ְ� ���� ����
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("InfiniteHighScore", bestScore);
            PlayerPrefs.Save();  // ��� ��ũ�� ����
        }

        // 4) UI ����
        ResultUI.SetActive(true);
        resultText.gameObject.SetActive(true);

        // �������� �׻� ������ ���� (���� ����, �ְ� ���)
        string scoreInfo = $"\n\nYour Score: {currentScore}\nBest Score: {bestScore}";

        if (result == GameResult.NoRemovableTiles)
        {
            resultText.text = colorTagStart + "No removable tiles!\nGame Over!" + scoreInfo + colorTagEnd;
        }
    }

    // ���� ȭ�� �ݱ�
    public void CloseMainUI()
    {
        MainUI.SetActive(false);
    }

    public void OnClickGoToMain()
    {
        GameEvents.OnGoToMainRequest?.Invoke(); // ���� ȭ������ �̵� �̺�Ʈ ȣ��
    }

    // ���� ȭ������ �̵�
    public void GoToMain()
    {
        MainUI.SetActive(true);
    }

    public void OnClickOpenOption()
    {
        GameEvents.OnOpenOption?.Invoke();
    }
}
