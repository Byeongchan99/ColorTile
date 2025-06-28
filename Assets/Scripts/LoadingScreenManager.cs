using System;
using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Animation Durations")]
    // Reveal : 1.417f + Hide : 1.5f = 2.917f => 1.4585f(재생 속도 반영)
    [Tooltip("‘Reveal’ 애니메이션이 끝나기까지 걸리는 시간(초) - 기존 1.417f")]
    [SerializeField] private float _revealDuration = 0.7085f; // Reveal 애니메이션이 끝나기까지 걸리는 시간(초)
    [Tooltip("‘Hide’ 애니메이션이 끝나기까지 걸리는 시간(초) - 기존 1.5f")]
    // Hide : 1.5f => 0.75f(재생 속도 반영)
    [SerializeField] private float _hideDuration = 0.75f; // Hide 애니메이션이 끝나기까지 걸리는 시간(초)
    [Tooltip("애니메이션 재생 속도")]
    //[SerializeField] private float _animationSpeed = 2f; // 애니메이션 재생 속도

    private Animator _animator;

    private bool _isLoading;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        GameEvents.OnGameStartedRequest += () => TryStartLoading(RevealLoadingScreen(GameEvents.OnGameStarted, _revealDuration));
        GameEvents.OnRetryGameRequest += () => TryStartLoading(RevealLoadingScreen(GameEvents.OnRetryGame, _revealDuration));
        GameEvents.OnClearBoardRequest += () => TryStartLoading(RevealLoadingScreen(GameEvents.OnClearBoard, _revealDuration));
        GameEvents.OnGoToMainRequest += () => TryStartLoading(RevealAndHideLoagindScreen(
                                                  GameEvents.OnGoToMainFirst,
                                                  GameEvents.OnGoToMainSecond,
                                                  _revealDuration,
                                                  _hideDuration));
    }

    private void TryStartLoading(IEnumerator loadingCoroutine)
    {
        if (_isLoading)
            return;            // 이미 로딩 중이면 무시

        StartCoroutine(LoadingFlow(loadingCoroutine));
    }

    private IEnumerator LoadingFlow(IEnumerator routine)
    {
        _isLoading = true;      // 여기서 “로딩 시작”
        yield return StartCoroutine(routine);
        _isLoading = false;     // 끝나면 “로딩 끝”
    }

    /// <summary>
    /// Reveal 트리거 → unscaled 시간 대기 → next 액션 호출
    /// </summary>
    public IEnumerator RevealLoadingScreen(Action nextEvent, float revealDuration)
    {
        _animator.SetTrigger("Reveal");
        yield return new WaitForSecondsRealtime(revealDuration);

        nextEvent?.Invoke();

        _animator.SetTrigger("Hide");
        // Hide 애니메이션이 끝날 때까지 대기
        yield return new WaitForSecondsRealtime(_hideDuration);
    }

    public IEnumerator RevealAndHideLoagindScreen(Action firstEvent, Action secondEvent, float revealDuration, float hideDuration)
    {
        _animator.SetTrigger("Reveal");
        yield return new WaitForSecondsRealtime(revealDuration);

        firstEvent?.Invoke();

        _animator.SetTrigger("Hide");
        yield return new WaitForSecondsRealtime(hideDuration);

        secondEvent?.Invoke();
    }
}
