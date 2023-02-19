using UnityEngine;

namespace HTH.AudioSystems
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_11
    /// 설명    : 전체 오디오 매니저
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance
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
        private static AudioManager _instance;
        [SerializeField] private GameObject _bgmAssets;
        [SerializeField] private GameObject _sfxAssets;
        [SerializeField] private GameObject _bgmPlayer;
        Sound currentBGM;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void PlayBGM(Sound sound)
        {
            if (sound == null)
                return;
            if (currentBGM != null)
                currentBGM.source.Stop();

            sound.source.Play();
            currentBGM = sound;
        }

        public void PlaySFX(Sound sound)
        {
            if (sound == null)
                return;
            sound.source.Play();
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Instantiate(_bgmAssets);
            Instantiate(_bgmPlayer);
            Instantiate(_sfxAssets);
        }
    }
}