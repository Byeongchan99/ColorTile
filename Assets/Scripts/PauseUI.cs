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

    // 1. 일시정지 화면 닫기
    public void ClosePauseUI()
    {
        gameObject.SetActive(false);
    }

    // 2. 게임 재시작
    public void RetryGame()
    {
        // 게임 재시작
    }

    // 3. 소리 On/Off
    public void SoundOnOff()
    {
        // 소리 On/Off
    }

    // 4. 진동 On/Off
    public void VibrationOnOff()
    {
        // 진동 On/Off
    }

    // 5. 메인 화면으로 이동
    public void GoToMain()
    {
        // 메인 화면으로 이동
    }
}
