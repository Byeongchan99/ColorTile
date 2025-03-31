using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public Button exitButton;
    public Button retryButton;
    public Button soundButton;
    public Button vibrationButton;
    public Button mainButton;

    // 1. �Ͻ����� ȭ�� �ݱ�
    public void ClosePauseUI()
    {
        gameObject.SetActive(false);
    }

    // 2. ���� �����
    public void RetryGame()
    {
        // ���� �����
    }

    // 3. �Ҹ� On/Off
    public void SoundOnOff()
    {
        // �Ҹ� On/Off
    }

    // 4. ���� On/Off
    public void VibrationOnOff()
    {
        // ���� On/Off
    }

    // 5. ���� ȭ������ �̵�
    public void GoToMain()
    {
        // ���� ȭ������ �̵�
    }
}
