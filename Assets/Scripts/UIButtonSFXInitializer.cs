using UnityEngine;
using UnityEngine.UI;

[Tooltip("모든 버튼 컴포넌트를 찾아서 효과음을 재생하는 OnClick 이벤트를 추가")]
public class UIButtonSFXInitializer : MonoBehaviour
{
    [Tooltip("모든 버튼 클릭 시 Invoke할 SFX 인덱스")]
    public int sfxIndex = 0;

    void Start()
    {
        // 씬 내 모든 Button 컴포넌트를 찾아서
        var buttons = FindObjectsOfType<Button>();
        foreach (var button in buttons)
        {
            // 클로저 문제가 없도록 로컬 변수에 복사
            int idx = sfxIndex;

            // onClick에 바로 이벤트 호출 추가
            button.onClick.AddListener(() =>
            {
                GameEvents.OnPlaySFX?.Invoke(idx);
            });
        }
    }
}
