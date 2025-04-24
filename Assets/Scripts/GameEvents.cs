using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums; // Enums.cs���� ������ enum ���

public static class GameEvents
{
    // PlayManager
    public static Action<int> OnScoreChanged; // ���� ���� �� ȣ��
    public static Action<float> OnTimerUpdated; // Ÿ�̸� ������Ʈ �� ȣ��
    public static Action<GameResult> OnGameEnded; // ���� ���� �� ȣ��
    public static Action OnClearBoard; // ���� Ŭ���� �� ȣ��(���� ���)

    // StageGenerator
    public static Action OnGameStarted; // ���� ���� �� ȣ��
    public static Action OnPauseGame; // ���� �Ͻ����� �� ȣ��
    
    // PauseUI, GameManager
    public static Action OnResumeGame; // ���� �簳 �� ȣ��

    // GameManager, UIManager, ResultUI, PauseUI
    public static Action OnRetryGame; // ���� ����� �� ȣ��
    public static Action OnGoToMain; // ���� ȭ������ �̵� �� ȣ��

    // PauseUI, OptionUI
    public static Action OnOpenOption; // �ɼ� ȭ�� ����

    // OptionUI, AudioVibrationManager
    public static Action<bool> OnBGMChanged; // BGM ���� ���� �� ȣ��
    public static Action<bool> OnSFXChanged; // SFX ���� ���� �� ȣ��
    public static Action<bool> OnVibrationChanged; // ���� ���� ���� �� ȣ��

    // AudioVibrationManager
    public static Action<int> OnPlaySFX; // SFX ��� �� ȣ��
}
