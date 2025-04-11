using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button exitButton;
    public Button retryButton;
    public Button optionButton;
    public Button mainButton;

    void Awake()
    {
        // 버튼 클릭 이벤트 등록
        exitButton.onClick.AddListener(ClosePauseUI);
        retryButton.onClick.AddListener(RetryGame);
        optionButton.onClick.AddListener(OpenOption);
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

    public void OpenOption()
    {
        Debug.Log("Open Option UI");
        GameEvents.OnOpenOption?.Invoke();
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
