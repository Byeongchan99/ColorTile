using UnityEngine;
using UnityEngine.UI;

[Tooltip("��� ��ư ������Ʈ�� ã�Ƽ� ȿ������ ����ϴ� OnClick �̺�Ʈ�� �߰�")]
public class UIButtonSFXInitializer : MonoBehaviour
{
    [Tooltip("��� ��ư Ŭ�� �� Invoke�� SFX �ε���")]
    public int sfxIndex = 0;

    void Start()
    {
        // �� �� ��� Button ������Ʈ�� ã�Ƽ�
        var buttons = FindObjectsOfType<Button>();
        foreach (var button in buttons)
        {
            // Ŭ���� ������ ������ ���� ������ ����
            int idx = sfxIndex;

            // onClick�� �ٷ� �̺�Ʈ ȣ�� �߰�
            button.onClick.AddListener(() =>
            {
                GameEvents.OnPlaySFX?.Invoke(idx);
            });
        }
    }
}
