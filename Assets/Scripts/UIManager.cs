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

    // 초기화
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% 남은 시간
        scoreText.text = "0";
    }

    // 1. 타이머 UI 업데이트
    public void UpdateTimer(float value)
    {
        timerSlider.value = value;
    } 

    // 2. 점수 UI 업데이트
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

    // 3. 일시정지 버튼
    public void OnClickPauseButton()
    {
        gameManager.StopTime();
        PauseUI.SetActive(true); // 일시정지 UI 활성화
    }

    // 게임 종료 처리 (win: true면 승리, false면 패배)
    public void EndGame(bool win)
    {
        gameManager.StopTime();
        ShowResult(win);
        Debug.Log(win ? "Stage cleared! You win!" : "Time's up! Game Over!");
    }

    // 4. 게임 종료 결과 UI 표시
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

    // 5. 메인 화면 닫기
    public void CloseMainUI()
    {
        MainUI.SetActive(false);
    }

    // 6. 메인 화면으로 이동
    public void GoToMain()
    {
        gameManager.StopTime();
        MainUI.SetActive(true);
    }
}
