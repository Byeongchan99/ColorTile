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

    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();
       
        GameEvents.OnGameStartedRequest += () =>
           StartCoroutine(RevealLoadingScreen(GameEvents.OnGameStarted, _revealDuration));
        GameEvents.OnRetryGameRequest += () =>
           StartCoroutine(RevealLoadingScreen(GameEvents.OnRetryGame, _revealDuration));
        GameEvents.OnClearBoardRequest += () =>
           StartCoroutine(RevealLoadingScreen(GameEvents.OnClearBoard, _revealDuration));
        GameEvents.OnGoToMainRequest += () =>
           StartCoroutine(RevealAndHideLoagindScreen(GameEvents.OnGoToMainFirst, GameEvents.OnGoToMainSecond, _revealDuration, _hideDuration));
    }

    /// <summary>
    /// Reveal 트리거 → unscaled 시간 대기 → next 액션 호출
    /// </summary>
    public IEnumerator RevealLoadingScreen(Action nextEvent, float duration)
    {
        _animator.SetTrigger("Reveal");
        yield return new WaitForSecondsRealtime(duration);
        nextEvent?.Invoke();
        _animator.SetTrigger("Hide");
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
