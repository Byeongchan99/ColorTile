using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums; // Enums.cs에서 정의한 enum 사용

public static class GameEvents
{
    // PlayManager
    public static Action<int> OnScoreChanged; // 점수 변경 시 호출
    public static Action<float> OnTimerUpdated; // 타이머 업데이트 시 호출
    public static Action<GameResult> OnGameEnded; // 게임 종료 시 호출
    public static Action OnClearBoard; // 보드 클리어 시 호출(무한 모드)
    public static Action OnClearBoardRequest;

    // StageGenerator
    public static Action OnGameStarted;
    public static Action OnGameStartedRequest; // 게임 시작 시 호출
    public static Action OnPauseGame; // 게임 일시정지 시 호출
    
    // PauseUI, GameManager
    public static Action OnResumeGame; // 게임 재개 시 호출

    // GameManager, UIManager, ResultUI, PauseUI
    public static Action OnRetryGame; // 게임 재시작 시 호출
    public static Action OnRetryGameRequest;

    public static Action OnGoToMainFirst;
    public static Action OnGoToMainSecond;
    public static Action OnGoToMainRequest; // 메인 화면으로 이동 시 호출

    // PauseUI, OptionUI
    public static Action OnOpenOption; // 옵션 화면 열기

    // OptionUI, AudioVibrationManager
    public static Action<bool> OnBGMChanged; // BGM 설정 변경 시 호출
    public static Action<bool> OnSFXChanged; // SFX 설정 변경 시 호출
    public static Action<bool> OnVibrationChanged; // 진동 설정 변경 시 호출

    // AudioVibrationManager
    public static Action<int> OnPlaySFX; // SFX 재생 시 호출    
}
