using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HTH.AudioSystems
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_11
    /// 설명    : 배경음 실행 대기 시켜놓기
    /// </summary>
    public class BGMAssets : MonoBehaviour
    {
        public static BGMAssets instance
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
        private static BGMAssets _instance;
        [SerializeField] private AudioMixerGroup bgmMixerGroup;
        [SerializeField] private List<Sound> bgms = new List<Sound>();


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public static Sound GetBGM(string clipName) =>
            instance.bgms.Find(x => x.name == clipName);


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
            Setup();
        }

        private void Setup()
        {
            foreach (Sound sound in instance.bgms)
            {
                sound.source = instance.gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                sound.source.outputAudioMixerGroup = instance.bgmMixerGroup;
            }
        }
    }
}
