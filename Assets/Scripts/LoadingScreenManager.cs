using System;
using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Animation Durations")]
    [Tooltip("‘Reveal’ 애니메이션이 끝나기까지 걸리는 시간(초)")]
    // reveal : 1.417f, hide : 1.5f 2.917f, 1.4585f

    private Animator _animator;

    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();
       
        GameEvents.OnGameStartedRequest += () =>
           StartCoroutine(RevealLoadingScreen(GameEvents.OnGameStarted, 0.7085f));
        GameEvents.OnRetryGameRequest += () =>
           StartCoroutine(RevealLoadingScreen(GameEvents.OnRetryGame, 0.7085f));
        GameEvents.OnClearBoardRequest += () =>
           StartCoroutine(RevealLoadingScreen(GameEvents.OnClearBoard, 0.7085f));
        GameEvents.OnGoToMainRequest += () =>
           StartCoroutine(RevealAndHideLoagindScreen(GameEvents.OnGoToMainFirst, GameEvents.OnGoToMainSecond, 0.7085f, 0.75f));
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
