using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button exitButton;
    public Button retryButton;

    private void Awake()
    {
        retryButton.onClick.AddListener(RetryGame);
        exitButton.onClick.AddListener(GoToMain);
        //exitButton.onClick.AddListener(() => GameEvents.OnGoToMain?.Invoke());
        //retryButton.onClick.AddListener(() => GameEvents.OnRetryGame?.Invoke());
    }

    // ���� �����
    public void RetryGame()
    {
        GameEvents.OnRetryGameRequest?.Invoke();
        this.gameObject.SetActive(false);
    }

    // ���� ȭ������ �̵�
    public void GoToMain()
    {
        GameEvents.OnGoToMainRequest?.Invoke();
        this.gameObject.SetActive(false);
    }
}
