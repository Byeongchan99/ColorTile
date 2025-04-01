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
    public bool isTimerOn = false; // 타이머 사용 여부
    public float playTime; // 게임 시간 (초)
    public float timeRemaining; // 남은 시간
    public float penaltyTime = 5f; // 틀린 클릭 시 감점 시간

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
            // 타이머 업데이트
            timeRemaining -= Time.deltaTime;
            timerSlider.value = timeRemaining / playTime;

            if (timeRemaining <= 0)
            {
                EndGame(false);
            }
        }
    }

    // 초기화
    public void Initialize()
    {
        resultText.gameObject.SetActive(false);
        timerSlider.value = 1f; // 100% 남은 시간
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

    // 2. 점수 UI 업데이트
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    // 3. 일시정지 버튼
    public void OnClickPauseButton()
    {
        Time.timeScale = 0f; // 게임 일시정지
        isTimerOn = false; // 타이머 정지
        PauseUI.SetActive(true); // 일시정지 UI 활성화
    }

    // 게임 종료 처리 (win: true면 승리, false면 패배)
    public void EndGame(bool win)
    {
        ShowResult(win);
        Debug.Log(win ? "Stage cleared! You win!" : "Time's up! Game Over!");
        enabled = false;
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
        Time.timeScale = 1f;
    }

    // 6. 메인 화면으로 이동
    public void GoToMain()
    {
        Time.timeScale = 0f;
        MainUI.SetActive(true);
    }
}
