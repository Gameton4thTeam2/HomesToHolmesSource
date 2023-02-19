using UnityEngine;
using UnityEngine.UI;
using HTH.DataModels;

namespace HTH.UI
{
    /// <summary>
    /// 작성자      : 권병석
    /// 작성일      : 2023_01_13
    /// 설명        : 세팅 UI
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_13
    /// 설명        : Awake lock 설정 [2023_01_23, 조영민]
    ///               감도 설정 및 개인정보처리방침 추가
    /// </summary>
    public class SettingsUI : UIMonoBehaviour<SettingsUI>
    {
        private SettingsData _settingsData;
        private string _privacyPolicyURL = "https://sites.google.com/view/homestoholmes-privacypolicy/";
        [SerializeField] private Slider _bgmVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Image _bgmOnOffImage;
        [SerializeField] private Image _sfxOnOffImage;
        [SerializeField] private Sprite _onSoundSprite;
        [SerializeField] private Sprite _offSoundSprite;
        [SerializeField] private Slider _controlSensitivitySlider;
        [SerializeField] private Button _closeSettings;
        [SerializeField] private Button _naverBlog;
        [SerializeField] private Button _twitter;
        [SerializeField] private Button _instagram;
        [SerializeField] private Button _privacyPolicy;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public override void Show()
        {
            _settingsData.onChangedBGM += OnValueChangeBGMVolumeSlider;
            _settingsData.onChangedSFX += OnValueChangeSFXVolumeSlider;
            _settingsData.onChangedSensitivity += OnValueChangeControlSensitivitySlider;
            base.Show();
        }

        public override void Hide()
        {
            PlayerPrefs.Save();
            _settingsData.onChangedBGM -= OnValueChangeBGMVolumeSlider;
            _settingsData.onChangedSFX -= OnValueChangeSFXVolumeSlider;
            _settingsData.onChangedSensitivity -= OnValueChangeControlSensitivitySlider;
            base.Hide();
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        override protected void Init()
        {
            _settingsData = SettingsData.instance;
            _closeSettings.onClick.AddListener(Hide);
            _naverBlog.onClick.AddListener(() =>
            {
                Application.OpenURL("https://blog.naver.com/homes_to_holmes");
            });
            _twitter.onClick.AddListener(() =>
            {
                Application.OpenURL("https://twitter.com/homes_to_holmes");
            });
            _instagram.onClick.AddListener(() =>
            {
                Application.OpenURL("https://instagram.com/homes_to_holmes");
            });
            _privacyPolicy.onClick.AddListener(() =>
            {
                Application.OpenURL(_privacyPolicyURL);
            });
            base.Init();
        }
        

        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void OnValueChangeBGMVolumeSlider(float value)
        {
            _bgmVolumeSlider.value = value;
            if (value != 0) 
                _bgmOnOffImage.sprite = _onSoundSprite;
            else
                _bgmOnOffImage.sprite = _offSoundSprite;
        }

        private void OnValueChangeSFXVolumeSlider(float value)
        {
            _sfxVolumeSlider.value = value;
            if (value != 0)
                _sfxOnOffImage.sprite = _onSoundSprite;
            else
                _sfxOnOffImage.sprite = _offSoundSprite;
        }

        private void OnValueChangeControlSensitivitySlider(float value)
        {
            _controlSensitivitySlider.value = value;
        }
    }
}