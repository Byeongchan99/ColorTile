using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider BGMSlider;
    public Slider SFXSlider;

    private void Awake()
    {
        GameEvents.OnOpenOption += OpenOption; // �ɼ� ���� �̺�Ʈ ���
        GameEvents.OnCloseOption += CloseOption; // �ɼ� �ݱ� �̺�Ʈ ���

        // �����̴� �� ���� �̺�Ʈ ���
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);
    }

    public void OpenOption()
    {
        this.gameObject.SetActive(true); // �ɼ� UI Ȱ��ȭ
    }

    public void CloseOption()
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
