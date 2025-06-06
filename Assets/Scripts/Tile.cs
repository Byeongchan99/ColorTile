using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite cbSprite;

    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null)
        {
            Debug.LogError("SpriteRenderer component not found on Tile.");
            return;
        }

        // 초기 모드 설정
        ApplyMode(Settings.GetBool(Settings.KEY_COLORBLIND_MODE));
        
        // 색약 모드 변경 이벤트 구독
        GameEvents.OnColorblindModeChanged += ApplyMode;
    }

    private void ApplyMode(bool isCb)
    {
        _sr.sprite = isCb ? cbSprite : normalSprite;
    }
}
