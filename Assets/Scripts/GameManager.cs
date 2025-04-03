using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
    public GameEventChannel eventChannel; // �̺�Ʈ ���

    void OnEnable()
    {
        eventChannel.onGameStart.AddListener(GameStart);
        eventChannel.onGameOver.AddListener(GameOver);
        eventChannel.onPause.AddListener(Pause);
        eventChannel.onGameRestart.AddListener(GameRestart);
        eventChannel.onGoToMain.AddListener(GoToMain);
    }

    void OnDisable()
    {
        eventChannel.onGameStart.RemoveListener(GameStart);
        eventChannel.onGameOver.RemoveListener(GameOver);
        eventChannel.onPause.RemoveListener(Pause);
        eventChannel.onGameRestart.RemoveListener(GameRestart);
        eventChannel.onGoToMain.RemoveListener(GoToMain);
    }
    */

    [Header("Time Settings")]
    public bool isPaused = true;

    [Header("References")]
    public UIManager uiManager;
    public PlayManager playManager;
    public StageGenerator stageGenerator;

    void Awake()
    {
        StopTime(); 
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
