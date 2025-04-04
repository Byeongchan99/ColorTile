using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    // PlayManager
    public static Action<int> OnScoreChanged; // ���� ���� �� ȣ��
    public static Action<float> OnTimerUpdated; // Ÿ�̸� ������Ʈ �� ȣ��
    public static Action<bool> OnGameEnded; // ���� ���� �� ȣ��

    // StageGenerator
    public static Action OnGameStarted; // ���� ���� �� ȣ��
    public static Action OnPauseGame; // ���� �Ͻ����� �� ȣ��
    // PauseUI, GameManager
    public static Action OnResumeGame; // ���� �簳 �� ȣ��

    // GameManager, UIManager, ResultUI, PauseUI
    public static Action OnRetryGame; // ���� ����� �� ȣ��
    public static Action OnGoToMain; // ���� ȭ������ �̵� �� ȣ��
}
