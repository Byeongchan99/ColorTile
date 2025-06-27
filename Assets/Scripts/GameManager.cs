using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums; // Enums.cs에서 정의한 enum 사용

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    public bool isPaused = true;
    public static GameMode gameMode; // 게임 모드 (Normal, Infinite)

    [SerializeField, Tooltip("목표 FPS (0 = 무제한)")]
    int targetFps = 60;

    void Awake()
    {
        StopTime();

        // 60프레임 설정
        // V Sync 완전히 해제
        QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate에 따라 프레임 고정
        Application.targetFrameRate = targetFps;
    }
 
    private void OnEnable()
    {
        GameEvents.OnGameStarted += StartTime; // 게임 시작 시 시간 재개
        GameEvents.OnResumeGame += StartTime; // 게임 재개 시 시간 재개
        GameEvents.OnRetryGame += StartTime; // 게임 재시작 시 시간 재개
        
        GameEvents.OnPauseGame += StopTime; // 게임 일시정지 시 시간 정지
        // OnGameEnded 이벤트는 bool 파라미터를 무시하고 StopTime() 실행
        GameEvents.OnGameEnded += (GameResult result) => StopTime(); // 게임 종료 시 시간 정지
        GameEvents.OnGoToMainSecond += StopTime; // 메인 화면으로 이동 시 시간 정지
    }
    

    // 게임 정지
    public void StopTime()
    {
        //Time.timeScale = 0f;
        isPaused = true;
    }

    // 게임 실행
    public void StartTime()
    {
        //Time.timeScale = 1f;
        isPaused = false;
    }
}
