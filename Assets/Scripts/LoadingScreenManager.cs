using System;
using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Animation Durations")]
    [Tooltip("‘Reveal’ 애니메이션이 끝나기까지 걸리는 시간(초)")]
    private float revealDuration = 0.75f;
    // reveal : 1.5f, hide : 1.417f

    private Animator _animator;

    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();

        GameEvents.OnGameStartedRequest += () =>
           StartCoroutine(DoReveal(GameEvents.OnGameStarted, 0.75f));
        GameEvents.OnRetryGameRequest += () =>
           StartCoroutine(DoReveal(GameEvents.OnRetryGame, 0.75f));
        GameEvents.OnClearBoardRequest += () =>
           StartCoroutine(DoReveal(GameEvents.OnClearBoard, 0.75f));
        GameEvents.OnGoToMainRequest += () =>
           StartCoroutine(DoReveal(GameEvents.OnGoToMain, 1.4585f));
    }

    public void RevealLoadingScreen()
    {
        _animator.SetTrigger("Reveal");
    }

    /// <summary>
    /// Reveal 트리거 → unscaled 시간 대기 → next 액션 호출
    /// </summary>
    public IEnumerator DoReveal(Action nextEvent, float duration)
    {
        _animator.SetTrigger("Reveal");
        yield return new WaitForSecondsRealtime(duration);
        nextEvent?.Invoke();
    }
}
