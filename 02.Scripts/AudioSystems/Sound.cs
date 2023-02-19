using UnityEngine;


namespace HTH.AudioSystems
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_11
    /// 설명    : 사운드마다 name(이름), clip(실행 할 음악파일), volume(음량), pitch(clip을 실행 속도), loop(반복 여부 => BGM은 ON시키기)
    ///           source(실행 할 컴포넌트 신경 안쓰셔도 됩니다!) 부여할 데이터 구조
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(0f, 1f)]
        public float pitch;
        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}