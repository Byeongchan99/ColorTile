using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Game Event Channel")]
public class GameEventChannel : ScriptableObject
{
    public UnityEvent onGameStart;
    public UnityEvent onGameOver;
    public UnityEvent onPause;
    public UnityEvent onGameRestart;
    public UnityEvent onGoToMain;

    public void RaiseGameStart() => onGameStart?.Invoke();
    public void RaiseGameOver() => onGameOver?.Invoke();
    public void RaisePause() => onPause?.Invoke();
    public void RaiseGameRestart() => onGameRestart?.Invoke();
    public void RaiseGoToMain() => onGoToMain?.Invoke();
}
