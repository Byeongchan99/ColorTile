using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public Button exitButton;
    public Button retryButton;
    public Button soundButton;
    public Button vibrationButton;
    public Button mainButton;

    // ���°� (UI ���� ��� Ȱ���� �� ����)
    private bool isSoundOn = true;
    private bool isVibrationOn = true;

    [Header("References")]
    public GameManager gameManager;
    public UIManager uiManager;

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
        gameManager.StartTime();
        gameObject.SetActive(false);
    }

    // 2. ���� �����
    public void RetryGame()
    {
        gameManager.StartTime();
        this.gameObject.SetActive(false);
        uiManager.StartGame();
    }

    // 3. �Ҹ� On/Off
    public void SoundOnOff()
    {
        // ���� ���� ��� (On�̸� Off, Off�̸� On)
        isSoundOn = !isSoundOn;
        // AudioListener�� ������ 0 �Ǵ� 1�� ����
        AudioListener.volume = isSoundOn ? 1f : 0f;
        // �ʿ��ϴٸ�, UI �̹����� �ؽ�Ʈ�� ������Ʈ�մϴ�.
        Debug.Log("Sound toggled: " + (isSoundOn ? "On" : "Off"));
    }

    // 4. ���� On/Off
    public void VibrationOnOff()
    {
        // ���� ����� ���¸� ���
        isVibrationOn = !isVibrationOn;
        // ���� ���� ȣ���� ���� �� �̺�Ʈ���� ���ǿ� ���� �����ϰ� �ϰ�,
        // ���⼭�� ���� ���� �����ϰų� UI�� ������Ʈ�մϴ�.
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
        Debug.Log("Vibration toggled: " + (isVibrationOn ? "On" : "Off"));
    }

    // 5. ���� ȭ������ �̵�
    public void GoToMain()
    {
        gameManager.StopTime();
        this.gameObject.SetActive(false);
        uiManager.GoToMain();
    }
}
