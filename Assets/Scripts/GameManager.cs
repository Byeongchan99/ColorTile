using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums; // Enums.cs에서 정의한 enum 사용

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    public bool isPaused = true;
    public static GameMode gameMode; // 게임 모드 (Normal, Infinite)

    void Awake()
    {
        StopTime(); 
    }

    private void OnEnable()
    {
        GameEvents.OnGameStarted += StartTime; // 게임 시작 시 시간 재개
        GameEvents.OnResumeGame += StartTime; // 게임 재개 시 시간 재개
        GameEvents.OnRetryGame += StartTime; // 게임 재시작 시 시간 재개
        GameEvents.OnPauseGame += StopTime; // 게임 일시정지 시 시간 정지
        // OnGameEnded 이벤트는 bool 파라미터를 무시하고 StopTime() 실행
        GameEvents.OnGameEnded += (GameResult result) => StopTime(); // 게임 종료 시 시간 정지
        GameEvents.OnGoToMain += StopTime; // 메인 화면으로 이동 시 시간 정지
    }

    // 시간 정지 - 일시정지
    public void StopTime()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    // 시간 재개
    public void StartTime()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
