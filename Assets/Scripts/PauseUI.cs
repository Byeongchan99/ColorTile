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

    // ���°� (UI ���� ��� Ȱ���� �� ����)
    private bool _isSoundOn = true;
    private bool _isVibrationOn = true;

    void Awake()
    {
        // ��ư Ŭ�� �̺�Ʈ ���
        exitButton.onClick.AddListener(ClosePauseUI);
        retryButton.onClick.AddListener(RetryGame);
        soundButton.onClick.AddListener(SoundOnOff);
        vibrationButton.onClick.AddListener(VibrationOnOff);
        mainButton.onClick.AddListener(GoToMain);
    }

    // 1. �Ͻ����� ȭ�� �ݱ� (���� �簳)
    public void ClosePauseUI()
    {
        //gameManager.StartTime();
        GameEvents.OnResumeGame?.Invoke();
        gameObject.SetActive(false);
    }

    // 2. ���� �����
    public void RetryGame()
    {
        //gameManager.StartTime();
        GameEvents.OnRetryGame?.Invoke();
        this.gameObject.SetActive(false);
        //uiManager.StartGame();
    }

    // 3. �Ҹ� On/Off
    public void SoundOnOff()
    {
        // ���� ���� ��� (On�̸� Off, Off�̸� On)
        _isSoundOn = !_isSoundOn;
        // AudioListener�� ������ 0 �Ǵ� 1�� ����
        AudioListener.volume = _isSoundOn ? 1f : 0f;
        // �ʿ��ϴٸ�, UI �̹����� �ؽ�Ʈ�� ������Ʈ�մϴ�.
        Debug.Log("Sound toggled: " + (_isSoundOn ? "On" : "Off"));
    }

    // 4. ���� On/Off
    public void VibrationOnOff()
    {
        // ���� ����� ���¸� ���
        _isVibrationOn = !_isVibrationOn;
        // ���� ���� ȣ���� ���� �� �̺�Ʈ���� ���ǿ� ���� �����ϰ� �ϰ�,
        // ���⼭�� ���� ���� �����ϰų� UI�� ������Ʈ�մϴ�.
        PlayerPrefs.SetInt("Vibration", _isVibrationOn ? 1 : 0);
        Debug.Log("Vibration toggled: " + (_isVibrationOn ? "On" : "Off"));
    }

    //civitai
    // 5. ���� ȭ������ �̵�
    public void GoToMain()
    {
        //gameManager.StopTime();
        GameEvents.OnGoToMain?.Invoke();
        this.gameObject.SetActive(false);
        //uiManager.GoToMain();
    }
}
