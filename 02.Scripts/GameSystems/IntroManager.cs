using UnityEngine;
using HTH.DataDependencySources;
using HTH.UI;

namespace HTH.GameSystems
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_02_01
    /// 설명    : 인트로를 관리하는 매니저 클래스
    /// 수정자  : 조영민
    /// 수정일  : 2023_02_11
    /// 수정내용 : 기존 스텝 제거 및 Firebase DB에 인트로 재생 내역 물어보고 인트로 실행하는 함수구현
    /// </summary>
    public class IntroManager : SingletonMonoBase<IntroManager>
    {
        public bool hasPlayed = false;
        public bool isFinished = false;
        [SerializeField] private ChatsInfo _startIntroChats;
        [SerializeField] private ChatsInfo _endStartIntroChats;
        

        public void Play()
        {
            if (hasPlayed)
                return;

            hasPlayed = true;

            if (User.nickName == "Guest")
            {
                IntroStartUI.instance.Show(_startIntroChats, null, () =>
                {
                    ChattingUI.instance.Show(_endStartIntroChats, null, () =>
                    {
                        isFinished = true;
                    });
                });
            }
            else
            {
                // 유저가 인트로 진행한적 없으면 인트로진행
                FirebaseManager.instance
                    .GetIntroHistoryAsync()
                    .OnGetIntroHistoryComplete((result) =>
                    {
                        if (result)
                        {
                            isFinished = true;
                        }
                        else
                        {
                            IntroStartUI.instance.Show(_startIntroChats, null, () =>
                            {
                                ChattingUI.instance.Show(_endStartIntroChats, null, () =>
                                {
                                    isFinished = true;
                                });
                            });
                        }
                    });
            }
        }

        protected override void Init()
        {
            base.Init();
        }
    }
}