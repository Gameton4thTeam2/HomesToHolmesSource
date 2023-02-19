using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using HTH.DataModels;
using TMPro;

namespace HTH.UI
{
    /// <summary>
    /// 작성자      : 권병석
    /// 작성일      : 2023_01_13
    /// 설명        : 세팅 데이터모델 컨트롤러
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_13
    /// 설명        : 컨트롤러에서 플레이어의 액션을 받도록 수정[2023_01_19, 권병석]
    ///               감도 설정 추가
    /// </summary>
    public class SettingsUIController : MonoBehaviour
    {
        private SettingsData _settingsData;
        [SerializeField] private Slider _bgmVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private AudioMixer _mainMixer;
        [SerializeField] private Button _bgmOnOffButton;
        [SerializeField] private Button _sfxOnOffButton;
        [SerializeField] private Button _changeLanguageKorea;
        [SerializeField] private Button _changeLanguageEnglish;
        [SerializeField] private Slider _controlSensitivitySlider;
        [SerializeField] private TMP_Text _controlSensitivityText;

        float originBGMVolume;
        float originSFXVolume;
        bool isMuteBGM;
        bool isMuteSFX;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void OnValueChangeBGMSlider(float value)
        {
            if(isMuteBGM) // Mute일때 슬라이더를 움직이면 OnBGM 버튼으로 변경
                isMuteBGM = false;
            _settingsData.soundBGMVolume = value;
            if (value > 0)
                _mainMixer.SetFloat(Constants.SETTINGS_BGM_MIXER_KEY, Mathf.Log10(value) * 20);
            else
            { // 슬라이더 값이 0이 되면 OffBGM 버튼으로 변경
                _mainMixer.SetFloat(Constants.SETTINGS_BGM_MIXER_KEY, -80.0f);
                isMuteBGM = true;
            }
        }

        public void OnValueChangeSFXSlider(float value)
        {
            if (isMuteSFX)
                isMuteSFX = false;
            _settingsData.soundSFXVolume = value;
            if (value > 0)
                _mainMixer.SetFloat(Constants.SETTINGS_SFX_MIXER_KEY, Mathf.Log10(value) * 20);
            else
            {
                _mainMixer.SetFloat(Constants.SETTINGS_SFX_MIXER_KEY, -80.0f);
                isMuteSFX = true;
            }
        }

        public void OnClickBGMOnOffButton()
        {
            if(isMuteBGM)
            {
                _bgmVolumeSlider.value = originBGMVolume;
                if (_bgmVolumeSlider.value > 0)
                    _mainMixer.SetFloat(Constants.SETTINGS_BGM_MIXER_KEY, Mathf.Log10(_bgmVolumeSlider.value) * 20);
                else
                    _mainMixer.SetFloat(Constants.SETTINGS_BGM_MIXER_KEY, -80.0f);
                isMuteBGM = false;
            }
            else
            {
                originBGMVolume = _bgmVolumeSlider.value;
                _bgmVolumeSlider.value = 0;
                _mainMixer.SetFloat(Constants.SETTINGS_BGM_MIXER_KEY, -80.0f);
                isMuteBGM = true;
            }
        }

        public void OnClickSFXOnOffButton()
        {
            if(isMuteSFX)
            {
                _sfxVolumeSlider.value = originSFXVolume;
                if (_sfxVolumeSlider.value > 0)
                    _mainMixer.SetFloat(Constants.SETTINGS_SFX_MIXER_KEY, Mathf.Log10(_sfxVolumeSlider.value) * 20);
                else
                    _mainMixer.SetFloat(Constants.SETTINGS_SFX_MIXER_KEY, -80.0f);
                isMuteSFX = false;

            }
            else
            {
                originSFXVolume = _sfxVolumeSlider.value;
                _sfxVolumeSlider.value = 0;
                _mainMixer.SetFloat(Constants.SETTINGS_SFX_MIXER_KEY, -80.0f);
                isMuteSFX = true;
            }
        }

        public void OnClickChangeLanguageKorea()
        {
            _settingsData.language = 0;
        }

        public void OnClickChangeLanguageEnglish()
        {
            _settingsData.language = 1;
        }

        public void OnValueChangeControlSensitivitySlider(float value)
        {
            _settingsData.soundSFXVolume = value;
            _controlSensitivityText.text = ((value - _controlSensitivitySlider.minValue) /
                                            (_controlSensitivitySlider.maxValue - _controlSensitivitySlider.minValue)
                                            * 100).ToString("N0");
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _settingsData = SettingsData.instance;
            _bgmVolumeSlider.onValueChanged.AddListener(OnValueChangeBGMSlider);
            _sfxVolumeSlider.onValueChanged.AddListener(OnValueChangeSFXSlider);
            _bgmOnOffButton.onClick.AddListener(OnClickBGMOnOffButton);
            _sfxOnOffButton.onClick.AddListener(OnClickSFXOnOffButton);
            _changeLanguageKorea.onClick.AddListener(OnClickChangeLanguageKorea);
            _changeLanguageEnglish.onClick.AddListener(OnClickChangeLanguageEnglish);
            _controlSensitivitySlider.onValueChanged.AddListener(OnValueChangeControlSensitivitySlider);
        }

        private void Start()
        {
            _settingsData.soundBGMVolume = _settingsData.soundBGMVolume;
            _settingsData.soundSFXVolume = _settingsData.soundSFXVolume;
            _settingsData.controlSensitivity = _settingsData.controlSensitivity;
            _controlSensitivityText.text = ((_settingsData.controlSensitivity - _controlSensitivitySlider.minValue) /
                                            (_controlSensitivitySlider.maxValue - _controlSensitivitySlider.minValue)
                                            * 100).ToString("N0");
        }
    }
}