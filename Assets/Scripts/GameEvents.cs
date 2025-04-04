using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    // PlayManager
    public static Action<int> OnScoreChanged; // 점수 변경 시 호출
    public static Action<float> OnTimerUpdated; // 타이머 업데이트 시 호출
    public static Action<bool> OnGameEnded; // 게임 종료 시 호출

    // StageGenerator
    public static Action OnGameStarted; // 게임 시작 시 호출
    public static Action OnPauseGame; // 게임 일시정지 시 호출
    // PauseUI, GameManager
    public static Action OnResumeGame; // 게임 재개 시 호출

    // GameManager, UIManager, ResultUI, PauseUI
    public static Action OnRetryGame; // 게임 재시작 시 호출
    public static Action OnGoToMain; // 메인 화면으로 이동 시 호출
}
