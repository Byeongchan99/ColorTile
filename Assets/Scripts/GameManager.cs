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

    [Header("References")]
    public UIManager uiManager;
    public PlayManager playManager;
    public StageGenerator stageGenerator;

    // 1. ���� ����
    public void GameStart()
    {
        // 1. �������� ����
        stageGenerator.GenerateStage();
        // 2. Ÿ�̸� ����
        uiManager.StartGame();
        // 3. ����ȭ�� �ݱ�
        uiManager.CloseMainUI();
    }

    // 2. ���� ����
    public void GameOver()
    {
        // 1. Ÿ�̸� ����
        // 2. ���� ��� ǥ��
    }

    // 3. �Ͻ�����
    public void Pause()
    {
        // 1. Ÿ�̸� ����
        // 2. �Ͻ����� ȭ�� ǥ��
    }

    // 4. ���� �����
    public void GameRestart()
    {
        // 1. Ÿ�̸� �ʱ�ȭ �� �����
        // 2. �������� �ʱ�ȭ �� �����
        // 3. �Ͻ����� ȭ�� �ݱ�
    }

    // 5. ���� ȭ������ �̵�
    public void GoToMain()
    {
        // 1. Ÿ�̸� �ʱ�ȭ
        // 2. �������� �ʱ�ȭ
        // 3. ���� ��� ȭ�� / �Ͻ����� ȭ�� �ݱ�
        // 4. ���� ȭ�� ǥ��
    }
}
