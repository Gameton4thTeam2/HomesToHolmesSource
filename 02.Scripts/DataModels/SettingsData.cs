using System;
using UnityEngine;
using HTH.UI;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자      : 권병석
    /// 작성일      : 2023_01_13
    /// 설명        : 세팅 데이터모델
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_13
    /// 설명        : Localization을 위해 language 수정 [2023_01_19, 권병석]
    ///               구글 로그인 여부를 위한 변수 추가 => 0 : 로그인 한적 없음, 1 : 로그인 했었음 [2023_01_26, 권병석]
    ///               감소 설정 추가
    /// </summary>
    public class SettingsData
    {
        public event Action<float> onChangedBGM;
        public event Action<float> onChangedSFX;
        public event Action<float> onChangedSensitivity;

        public static SettingsData instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = Load();
                }
                return _instance;
            }
        }
        private static SettingsData _instance;

        public int isPlayerPrefsExistData
        {
            get
            {
                return PlayerPrefs.GetInt("PlayerPrefsExistData", _isPlayerPrefsExistData);
            }
            set
            {
                _isPlayerPrefsExistData = value;
                PlayerPrefs.SetInt("PlayerPrefsExistData", value);
                PlayerPrefs.Save();
            }
        }
        private int _isPlayerPrefsExistData;

        public int hasGoogleLoginData
        {
            get
            {
                return PlayerPrefs.GetInt("HasGoogleLoginData", _hasGoogleLoginData);
            }
            set
            {
                _hasGoogleLoginData = value;
                PlayerPrefs.SetInt("HasGoogleLoginData", value);
                PlayerPrefs.Save();
            }
        }
        private int _hasGoogleLoginData;

        public int language
        {
            get
            {
                return PlayerPrefs.GetInt("Language", _language);
            }
            set
            {
                _language = value;
                PlayerPrefs.SetInt("Language", _language);
                Localization.instance.language = _language;
                PlayerPrefs.Save();
            }
        }
        private int _language;

        public float soundBGMVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("BGMVolume", _soundBGMVolume);
            }
            set
            {
                _soundBGMVolume = value;
                PlayerPrefs.SetFloat("BGMVolume", _soundBGMVolume);
                onChangedBGM?.Invoke(_soundBGMVolume);
            }
        }
        private float _soundBGMVolume;

        public float soundSFXVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("SFXVolume", _soundSFXVolume);
            }
            set
            {
                _soundSFXVolume = value;
                PlayerPrefs.SetFloat("SFXVolume", _soundSFXVolume);
                onChangedSFX?.Invoke(_soundSFXVolume);
            }
        }
        private float _soundSFXVolume;

        public float controlSensitivity
        {
            get
            {
                return PlayerPrefs.GetFloat("Sensitivity", _controlSensitivity);
            }
            set
            {
                _controlSensitivity = value;
                PlayerPrefs.SetFloat("Sensitivity", _controlSensitivity);
                onChangedSensitivity?.Invoke(_controlSensitivity);
            }
        }
        private float _controlSensitivity;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public static SettingsData Load()
        {
            SettingsData data = new SettingsData();

            if (!PlayerPrefs.HasKey("PlayerPrefsExistData"))
                data.SetupFirstTIme();
            data.hasGoogleLoginData = PlayerPrefs.GetInt("HasGoogleLoginData");
            data.language = PlayerPrefs.GetInt("Language");
            data.soundBGMVolume = PlayerPrefs.GetFloat("BGMVolume");
            data.soundSFXVolume = PlayerPrefs.GetFloat("SFXVolume");
            data.controlSensitivity = PlayerPrefs.GetFloat("Sensitivity");

            return data;
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void SetupFirstTIme()
        {
            hasGoogleLoginData = 0;
            isPlayerPrefsExistData = 1;
            language = (int)Application.systemLanguage == 23 ? 
                    Constants.SETTINGS_SETUP_FIRST_TIME_LANGUAGE : Constants.SETTINGS_SETUP_FIRST_TIME_LANGUAGE + 1;
            soundBGMVolume = Constants.SETTINGS_SETUP_FIRST_TIME_SOUNDBGMVOLUME;
            soundSFXVolume = Constants.SETTINGS_SETUP_FIRST_TIME_SOUNDSFXVOLUME;
            controlSensitivity = Constants.SETTINGS_SETUP_FIRST_TIME_SENSITIVITY;
        }
    }
}