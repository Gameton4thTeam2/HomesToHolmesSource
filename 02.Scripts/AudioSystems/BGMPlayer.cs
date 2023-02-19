using System.Collections;
using UnityEngine;

namespace HTH.AudioSystems
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_11
    /// 설명    : 씬이나 게임 스테이지마다 자동 배경음악 플레이어
    /// </summary>
    public class BGMPlayer : MonoBehaviour
    {
        //===========================================================================
        //                             Private Methods
        //===========================================================================
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            StartCoroutine(E_StartBGM());
        }

        IEnumerator E_StartBGM()
        {
            yield return new WaitUntil(() => AudioManager.instance != null && BGMAssets.instance != null);

            Sound sound = null;

            sound = BGMAssets.GetBGM("LoginBGM");
            if (sound != null)
                AudioManager.instance.PlayBGM(sound);
        }
    }
}