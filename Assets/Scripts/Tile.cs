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

        // �ʱ� ��� ����
        ApplyMode(Settings.GetBool(Settings.KEY_COLORBLIND_MODE));
        
        // ���� ��� ���� �̺�Ʈ ����
        GameEvents.OnColorblindModeChanged += ApplyMode;
    }

    private void ApplyMode(bool isCb)
    {
        _sr.sprite = isCb ? cbSprite : normalSprite;
    }
}
