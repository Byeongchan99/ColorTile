using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button exitButton;
    public Button retryButton;

    [Header("References")]
    public GameManager gameManager;
    public UIManager uiManager;

    private void Awake()
    {
        exitButton.onClick.AddListener(GoToMain);
        retryButton.onClick.AddListener(RetryGame);
    }

    // 게임 재시작
    public void RetryGame()
    {
        gameManager.StartTime();
        this.gameObject.SetActive(false);
        uiManager.StartGame();
    }

    // 메인 화면으로 이동
    public void GoToMain()
    {
        gameManager.StopTime();
        this.gameObject.SetActive(false);
        uiManager.GoToMain();
    }
}
