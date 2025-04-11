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
        GameEvents.OnOpenOption += OpenOption; // 옵션 열기 이벤트 등록
        
        closeButton.onClick.AddListener(OnClickCloseOption); // 닫기 버튼 클릭 이벤트 등록

        // 슬라이더 값 변경 이벤트 등록
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false); // 옵션 UI 비활성화
        }
    }

    public void OpenOption()
    {
        Debug.Log("Option UI Opened");
        this.gameObject.SetActive(true); // 옵션 UI 활성화
    }

    public void OnClickCloseOption()
    {
        this.gameObject.SetActive(false); // 옵션 UI 비활성화
    }

    // BGM 슬라이더 값 변경 시 호출되는 메서드
    private void OnBGMSliderValueChanged(float value)
    {
        // BGM 볼륨 조정
        //AudioManager.Instance.SetBGMVolume(value);
        Debug.Log("BGM Volume: " + value);
    }

    // SFX 슬라이더 값 변경 시 호출되는 메서드
    private void OnSFXSliderValueChanged(float value)
    {
        // SFX 볼륨 조정
        //AudioManager.Instance.SetSFXVolume(value);
        Debug.Log("SFX Volume: " + value);
    }
}
