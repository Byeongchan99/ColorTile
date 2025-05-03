using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button exitButton; // ���� ȭ������ �̵� ��ư
    public Button retryButton; // ���� ����� ��ư

    private void Awake()
    {
        retryButton.onClick.AddListener(OnClickRetryGame);
        exitButton.onClick.AddListener(OnClickGoToMain);       
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
