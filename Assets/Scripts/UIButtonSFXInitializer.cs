using UnityEngine;
using UnityEngine.UI;

[Tooltip("��� ��ư ������Ʈ�� ã�Ƽ� ȿ������ ����ϴ� OnClick �̺�Ʈ�� �߰�")]
public class UIButtonSFXInitializer : MonoBehaviour
{
    [Tooltip("��� ��ư Ŭ�� �� Invoke�� SFX �ε���")]
    public int sfxIndex = 0;

    void Awake()
    {
        // �� �� ��� Button ������Ʈ�� ã�Ƽ�(��Ȱ��ȭ�� ������Ʈ ����)
        var buttons = FindObjectsOfType<Button>(includeInactive: true);

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
