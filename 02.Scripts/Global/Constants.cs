using System;

/// <summary>
/// 작성자  : 권병석
/// 작성일  : 2023_01_12
/// 설명    : 사용할 상수 모음집
/// </summary>
namespace HTH
{
    public class Constants
    {
        public const int MAIN_MENU_PADDING = 30;
        public const int MAIN_MENU_BOX_HEIGHT = 120;
        public const float MAIN_MENU_FIRST_START_Y = 0f;
        public const float MAIN_MENU_ANIMATION_SPEED = 0.5f;

        /// <summary>
        /// 0 : Korea, 1 : English
        /// </summary>
        public const int SETTINGS_SETUP_FIRST_TIME_LANGUAGE = 0;
        public const float SETTINGS_SETUP_FIRST_TIME_SOUNDBGMVOLUME = 0.5f;
        public const float SETTINGS_SETUP_FIRST_TIME_SOUNDSFXVOLUME = 0.5f;
        public const float SETTINGS_SETUP_FIRST_TIME_SENSITIVITY = 0.005f;

        public const string SETTINGS_BGM_MIXER_KEY = "BGMVolume";
        public const string SETTINGS_SFX_MIXER_KEY = "SFXVolume";
    }
}