using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Button exitButton;
    public Button retryButton;

    public GameManager gameManager;
    public UIManager uiManager;

    private void Awake()
    {
        exitButton.onClick.AddListener(GoToMain);
        retryButton.onClick.AddListener(RetryGame);
    }

    // 2. ���� �����
    public void RetryGame()
    {
        gameManager.StartTime();
        this.gameObject.SetActive(false);
        uiManager.StartGame();
    }

    // 5. ���� ȭ������ �̵�
    public void GoToMain()
    {
        gameManager.StopTime();
        this.gameObject.SetActive(false);
        uiManager.GoToMain();
    }
}
