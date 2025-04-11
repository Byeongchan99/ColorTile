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
        // ��ư Ŭ�� �̺�Ʈ ���
        exitButton.onClick.AddListener(ClosePauseUI);
        retryButton.onClick.AddListener(RetryGame);
        optionButton.onClick.AddListener(OpenOption);
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

    public void OpenOption()
    {
        Debug.Log("Open Option UI");
        GameEvents.OnOpenOption?.Invoke();
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
