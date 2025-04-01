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

    // 상태값 (UI 갱신 등에도 활용할 수 있음)
    private bool isSoundOn = true;
    private bool isVibrationOn = true;

    [Header("References")]
    public GameManager gameManager;
    public StageGenerator stageGenerator;
    public UIManager uiManager;

    void Start()
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
        // 게임 재개: 일시정지 상태 해제
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    // 2. 게임 재시작
    public void RetryGame()
    {
        // 재시작 전 반드시 일시정지 해제
        Time.timeScale = 1f;
        uiManager.isTimerOn = true;
        this.gameObject.SetActive(false);
        stageGenerator.GenerateStage();


        // 현재 씬을 다시 로드하여 게임을 리셋
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 3. 소리 On/Off
    public void SoundOnOff()
    {
        // 현재 상태 토글 (On이면 Off, Off이면 On)
        isSoundOn = !isSoundOn;
        // AudioListener의 볼륨을 0 또는 1로 설정
        AudioListener.volume = isSoundOn ? 1f : 0f;
        // 필요하다면, UI 이미지나 텍스트도 업데이트합니다.
        Debug.Log("Sound toggled: " + (isSoundOn ? "On" : "Off"));
    }

    // 4. 진동 On/Off
    public void VibrationOnOff()
    {
        // 진동 기능의 상태를 토글
        isVibrationOn = !isVibrationOn;
        // 실제 진동 호출은 게임 내 이벤트에서 조건에 따라 수행하게 하고,
        // 여기서는 설정 값만 저장하거나 UI를 업데이트합니다.
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
        Debug.Log("Vibration toggled: " + (isVibrationOn ? "On" : "Off"));
    }

    // 5. 메인 화면으로 이동
    public void GoToMain()
    {
        // 메인 화면으로 이동하기 전, 일시정지를 해제
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
        uiManager.GoToMain();

        // 빌드 설정에 추가된 메인 메뉴 씬의 이름(예: "MainMenu")으로 이동합니다.
        //SceneManager.LoadScene("MainMenu");
    }
}
