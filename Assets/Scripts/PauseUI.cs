using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button exitButton;
    public Button retryButton;
    public Button soundButton;
    public Button vibrationButton;
    public Button mainButton;

    // 상태값 (UI 갱신 등에도 활용할 수 있음)
    private bool _isSoundOn = true;
    private bool _isVibrationOn = true;

    void Awake()
    {
        // 버튼 클릭 이벤트 등록
        exitButton.onClick.AddListener(ClosePauseUI);
        retryButton.onClick.AddListener(RetryGame);
        soundButton.onClick.AddListener(SoundOnOff);
        vibrationButton.onClick.AddListener(VibrationOnOff);
        mainButton.onClick.AddListener(GoToMain);
    }

    // 1. 일시정지 화면 닫기 (게임 재개)
    public void ClosePauseUI()
    {
        //gameManager.StartTime();
        GameEvents.OnResumeGame?.Invoke();
        gameObject.SetActive(false);
    }

    // 2. 게임 재시작
    public void RetryGame()
    {
        //gameManager.StartTime();
        GameEvents.OnRetryGame?.Invoke();
        this.gameObject.SetActive(false);
        //uiManager.StartGame();
    }

    // 3. 소리 On/Off
    public void SoundOnOff()
    {
        // 현재 상태 토글 (On이면 Off, Off이면 On)
        _isSoundOn = !_isSoundOn;
        // AudioListener의 볼륨을 0 또는 1로 설정
        AudioListener.volume = _isSoundOn ? 1f : 0f;
        // 필요하다면, UI 이미지나 텍스트도 업데이트합니다.
        Debug.Log("Sound toggled: " + (_isSoundOn ? "On" : "Off"));
    }

    // 4. 진동 On/Off
    public void VibrationOnOff()
    {
        // 진동 기능의 상태를 토글
        _isVibrationOn = !_isVibrationOn;
        // 실제 진동 호출은 게임 내 이벤트에서 조건에 따라 수행하게 하고,
        // 여기서는 설정 값만 저장하거나 UI를 업데이트합니다.
        PlayerPrefs.SetInt("Vibration", _isVibrationOn ? 1 : 0);
        Debug.Log("Vibration toggled: " + (_isVibrationOn ? "On" : "Off"));
    }

    //civitai
    // 5. 메인 화면으로 이동
    public void GoToMain()
    {
        //gameManager.StopTime();
        GameEvents.OnGoToMain?.Invoke();
        this.gameObject.SetActive(false);
        //uiManager.GoToMain();
    }
}
