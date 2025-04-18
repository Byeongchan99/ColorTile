using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums; // Enums.cs���� ������ enum ���

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    public bool isPaused = true;
    public static GameMode gameMode; // ���� ��� (Normal, Infinite)

    void Awake()
    {
        StopTime(); 
    }

    private void OnEnable()
    {
        GameEvents.OnGameStarted += StartTime; // ���� ���� �� �ð� �簳
        GameEvents.OnResumeGame += StartTime; // ���� �簳 �� �ð� �簳
        GameEvents.OnRetryGame += StartTime; // ���� ����� �� �ð� �簳
        GameEvents.OnPauseGame += StopTime; // ���� �Ͻ����� �� �ð� ����
        // OnGameEnded �̺�Ʈ�� bool �Ķ���͸� �����ϰ� StopTime() ����
        GameEvents.OnGameEnded += (GameResult result) => StopTime(); // ���� ���� �� �ð� ����
        GameEvents.OnGoToMain += StopTime; // ���� ȭ������ �̵� �� �ð� ����
    }

    // �ð� ���� - �Ͻ�����
    public void StopTime()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    // �ð� �簳
    public void StartTime()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
