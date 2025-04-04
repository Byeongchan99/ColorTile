using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text resultText;
    public Slider timerSlider;
    public TMP_Text scoreText;

    public GameObject PauseUI;
    public GameObject MainUI;
    public GameObject ResultUI;

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
        scoreText.text = score.ToString();
    }

    // ���� ���� ��ư Ŭ��
    public void OnClickStartGameButton()
    {
        GameEvents.OnGameStarted?.Invoke(); // ���� ���� �̺�Ʈ ȣ��
        //gameManager.StartTime();
        MainUI.SetActive(false);
        StartGame();
    }

    // ���� ���� ó��
    public void StartGame()
    {
        resultText.gameObject.SetActive(false);
        ResultUI.SetActive(false);
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
    public void EndGame(bool win)
    {
        //gameManager.StopTime();
        ShowResult(win);
        Debug.Log(win ? "Stage cleared! You win!" : "Time's up! Game Over!");
    }

    // ���� ���� ��� UI ǥ��
    public void ShowResult(bool win)
    {
        ResultUI.SetActive(true);
        resultText.gameObject.SetActive(true);
        if (win)
        {
            resultText.text = "Stage cleared!\nYou win!\nScore: " + playManager.Score;
        }
        else
        {
            resultText.text = "Time's up!\nGame Over!\nScore: " + playManager.Score;
        }
    } 

    // ���� ȭ�� �ݱ�
    public void CloseMainUI()
    {
        MainUI.SetActive(false);
    }

    // ���� ȭ������ �̵�
    public void GoToMain()
    {
        gameManager.StopTime();
        //MainUI.SetActive(true);
    }
}
