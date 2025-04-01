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

    [Header("Game Timer Settings")]
    public bool isTimerOn = false; // Ÿ�̸� ��� ����
    public float playTime; // ���� �ð� (��)
    public float timeRemaining; // ���� �ð�
    public float penaltyTime = 5f; // Ʋ�� Ŭ�� �� ���� �ð�

    [Header("References")]
    public PlayManager playManager;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (isTimerOn)
        {
            // Ÿ�̸� ������Ʈ
            timeRemaining -= Time.deltaTime;
            timerSlider.value = timeRemaining / playTime;

            if (timeRemaining <= 0)
            {
                EndGame(false);
            }
        }
    }

    // �ʱ�ȭ
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% ���� �ð�
        isTimerOn = false;
        scoreText.text = "0";
    }

    public void StartGame()
    {
        isTimerOn = true;
        timeRemaining = playTime;
    }

    public void GetPenaltiy()
    {
        timeRemaining -= penaltyTime;
    }    

    // 2. ���� UI ������Ʈ
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    // 3. �Ͻ����� ��ư
    public void OnClickPauseButton()
    {
        Time.timeScale = 0f; // ���� �Ͻ�����
        isTimerOn = false; // Ÿ�̸� ����
        PauseUI.SetActive(true); // �Ͻ����� UI Ȱ��ȭ
    }

    // ���� ���� ó�� (win: true�� �¸�, false�� �й�)
    public void EndGame(bool win)
    {
        ShowResult(win);
        Debug.Log(win ? "Stage cleared! You win!" : "Time's up! Game Over!");
        enabled = false;
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
        Time.timeScale = 1f;
    }

    // 6. ���� ȭ������ �̵�
    public void GoToMain()
    {
        Time.timeScale = 0f;
        MainUI.SetActive(true);
    }
}
