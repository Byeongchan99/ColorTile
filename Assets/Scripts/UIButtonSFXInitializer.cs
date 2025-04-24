using UnityEngine;
using UnityEngine.UI;

public class UIButtonSFXInitializer : MonoBehaviour
{
    [Tooltip("��� ��ư Ŭ�� �� Invoke�� SFX �ε���")]
    public int sfxIndex = 0;

    void Start()
    {
        // �� �� ��� Button ������Ʈ�� ã�Ƽ���
        var buttons = FindObjectsOfType<Button>();
        foreach (var btn in buttons)
        {
            // Ŭ���� ������ ������ ���� ������ ����
            int idx = sfxIndex;

            // onClick�� �ٷ� �̺�Ʈ ȣ�� �߰�
            btn.onClick.AddListener(() =>
            {
                GameEvents.OnPlaySFX?.Invoke(idx);
            });
        }
    }
}
