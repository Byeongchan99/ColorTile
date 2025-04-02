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

    [Header("References")]
    public PlayManager playManager;
    public GameManager gameManager;
    public StageGenerator stageGenerator;

    private void Awake()
    {
        Initialize();
    }

    // �ʱ�ȭ
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% ���� �ð�
        scoreText.text = "0";
    }

    // 1. Ÿ�̸� UI ������Ʈ
    public void UpdateTimer(float value)
    {
        timerSlider.value = value;
    } 

    // 2. ���� UI ������Ʈ
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void OnClickStartGameButton()
    {
        gameManager.StartTime();
        MainUI.SetActive(false);
        StartGame();
    }

    public void StartGame()
    {
        resultText.gameObject.SetActive(false);
        playManager.Initialize();
        Initialize();
        stageGenerator.GenerateStage();
    }

    // 3. �Ͻ����� ��ư
    public void OnClickPauseButton()
    {
        gameManager.StopTime();
        PauseUI.SetActive(true); // �Ͻ����� UI Ȱ��ȭ
    }

    // ���� ���� ó�� (win: true�� �¸�, false�� �й�)
    public void EndGame(bool win)
    {
        gameManager.StopTime();
        ShowResult(win);
        Debug.Log(win ? "Stage cleared! You win!" : "Time's up! Game Over!");
    }

    // 4. ���� ���� ��� UI ǥ��
    public void ShowResult(bool win)
    {
        resultText.gameObject.SetActive(true);
        if (win)
        {
            resultText.text = "Stage cleared!\nYou win!\nScore: " + playManager.score;
        }
        else
        {
            resultText.text = "Time's up!\nGame Over!\nScore: " + playManager.score;
        }
    } 

    // 5. ���� ȭ�� �ݱ�
    public void CloseMainUI()
    {
        MainUI.SetActive(false);
    }

    // 6. ���� ȭ������ �̵�
    public void GoToMain()
    {
        gameManager.StopTime();
        MainUI.SetActive(true);
    }
}
