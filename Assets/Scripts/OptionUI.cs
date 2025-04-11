using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Button closeButton;

    private void Awake()
    {
        GameEvents.OnOpenOption += OpenOption; // �ɼ� ���� �̺�Ʈ ���
        
        closeButton.onClick.AddListener(OnClickCloseOption); // �ݱ� ��ư Ŭ�� �̺�Ʈ ���

        // �����̴� �� ���� �̺�Ʈ ���
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false); // �ɼ� UI ��Ȱ��ȭ
        }
    }

    public void OpenOption()
    {
        Debug.Log("Option UI Opened");
        this.gameObject.SetActive(true); // �ɼ� UI Ȱ��ȭ
    }

    public void OnClickCloseOption()
    {
        this.gameObject.SetActive(false); // �ɼ� UI ��Ȱ��ȭ
    }

    // BGM �����̴� �� ���� �� ȣ��Ǵ� �޼���
    private void OnBGMSliderValueChanged(float value)
    {
        // BGM ���� ����
        //AudioManager.Instance.SetBGMVolume(value);
        Debug.Log("BGM Volume: " + value);
    }

    // SFX �����̴� �� ���� �� ȣ��Ǵ� �޼���
    private void OnSFXSliderValueChanged(float value)
    {
        // SFX ���� ����
        //AudioManager.Instance.SetSFXVolume(value);
        Debug.Log("SFX Volume: " + value);
    }
}
