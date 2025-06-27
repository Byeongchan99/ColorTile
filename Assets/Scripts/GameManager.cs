using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums; // Enums.cs���� ������ enum ���

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    public bool isPaused = true;
    public static GameMode gameMode; // ���� ��� (Normal, Infinite)

    [SerializeField, Tooltip("��ǥ FPS (0 = ������)")]
    int targetFps = 60;

    void Awake()
    {
        StopTime();

        // 60������ ����
        // V Sync ������ ����
        QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate�� ���� ������ ����
        Application.targetFrameRate = targetFps;
    }
 
    private void OnEnable()
    {
        GameEvents.OnGameStarted += StartTime; // ���� ���� �� �ð� �簳
        GameEvents.OnResumeGame += StartTime; // ���� �簳 �� �ð� �簳
        GameEvents.OnRetryGame += StartTime; // ���� ����� �� �ð� �簳
        
        GameEvents.OnPauseGame += StopTime; // ���� �Ͻ����� �� �ð� ����
        // OnGameEnded �̺�Ʈ�� bool �Ķ���͸� �����ϰ� StopTime() ����
        GameEvents.OnGameEnded += (GameResult result) => StopTime(); // ���� ���� �� �ð� ����
        GameEvents.OnGoToMainSecond += StopTime; // ���� ȭ������ �̵� �� �ð� ����
    }
    

    // ���� ����
    public void StopTime()
    {
        //Time.timeScale = 0f;
        isPaused = true;
    }

    // ���� ����
    public void StartTime()
    {
        //Time.timeScale = 1f;
        isPaused = false;
    }
}
