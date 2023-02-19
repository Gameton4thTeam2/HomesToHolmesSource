using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HTH.AudioSystems
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_11
    /// 설명    : 효과음 실행 대기 시켜놓기
    /// </summary>
    public class SFXAssets : MonoBehaviour
    {
        public static SFXAssets instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        private static SFXAssets _instance;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private List<Sound> sfxs = new List<Sound>();


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public static Sound GetSFX(string clipName) =>
            instance.sfxs.Find(x => x.name == clipName);


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

            }
        }

        private void Start()
        {
            SetUp();
        }

        private void SetUp()
        {
            foreach (Sound sound in sfxs)
            {
                sound.source = instance.gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                sound.source.outputAudioMixerGroup = instance.sfxMixerGroup;
            }
        }
    }
}