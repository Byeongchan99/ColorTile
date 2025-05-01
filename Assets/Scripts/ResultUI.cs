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
        retryButton.onClick.AddListener(OnClickRetryGame);
        exitButton.onClick.AddListener(OnClickGoToMain);
        //exitButton.onClick.AddListener(() => GameEvents.OnGoToMain?.Invoke());
        //retryButton.onClick.AddListener(() => GameEvents.OnRetryGame?.Invoke());
    }

    // ���� �����
    public void OnClickRetryGame()
    {
        GameEvents.OnRetryGameRequest?.Invoke();
        this.gameObject.SetActive(false);
    }

    // ���� ȭ������ �̵�
    public void OnClickGoToMain()
    {
        GameEvents.OnGoToMainRequest?.Invoke();
        this.gameObject.SetActive(false);
    }
}
