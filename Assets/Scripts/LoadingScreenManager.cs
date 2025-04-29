using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    private Animator _animatorComponent;

    private void Awake()
    {
        _animatorComponent = transform.GetComponent<Animator>();
        GameEvents.OnGameStarted += RevealLoadingScreen; // 게임 시작 시 초기화
        GameEvents.OnRetryGame += RevealLoadingScreen; // 게임 재시작 시 초기화
        GameEvents.OnClearBoard += RevealLoadingScreen;
        GameEvents.OnGoToMain += RevealLoadingScreen;
    }

    public void RevealLoadingScreen()
    {
        _animatorComponent.SetTrigger("Reveal");
    }
}
