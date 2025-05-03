using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button exitButton; // 메인 화면으로 이동 버튼
    public Button retryButton; // 게임 재시작 버튼

    private void Awake()
    {
        retryButton.onClick.AddListener(OnClickRetryGame);
        exitButton.onClick.AddListener(OnClickGoToMain);       
    }

    // 게임 재시작
    public void OnClickRetryGame()
    {
        GameEvents.OnRetryGameRequest?.Invoke();
        this.gameObject.SetActive(false);
    }

    // 메인 화면으로 이동
    public void OnClickGoToMain()
    {
        GameEvents.OnGoToMainRequest?.Invoke();
        this.gameObject.SetActive(false);
    }
}
