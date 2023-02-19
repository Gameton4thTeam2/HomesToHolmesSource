using HTH.DataModels;
using HTH.WorldElements;
using HTH.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using HTH.Assets;
using Firebase.Database;
using System.Collections.Generic;
using HTH.InputHandlers;

namespace HTH.GameSystems
{
    /// <summary>
    /// 작성자      : 조영민
    /// 작성일      : 2023_01_09
    /// 설명        : 전체 시스템플로우 매니저
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_09
    /// 설명        : 로그인 씬을 위한 데이터 준비 및 LoginUI를 Show하는 State 추가[2023_01_26, 권병석]
    ///               인트로를 시작 체크하는 State 추가[2023_02_01, 권병석]
    ///               인트로 실행 체크를 안드로이드 플랫폼에서만 동작하도록 수정
    /// </summary>
    public class GameManager : SingletonMonoBase<GameManager>
    {
        public enum State
        {
            Idle,
            Loggin,
            LoadLoginData,
            WaitUntilLoggedIn,
            LoadUserData,
            WaitUntilUserDataLoaded,
            CheckPlayerHistory,
            DoFirstTutorial,
            WaitUntilFirstTutorialFinished,
            StartGame,
            LoadGameObjects,
            WaitUntilGameObjectsLoaded,
            InitializeObjectPools,            
            StartIntro,
            WaitUntilIntroFinished,
            DefaultSetUp,
            InGame,
        }
        public State current;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void ChangeState(State newState) => current = newState;


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            base.Init();
            DontDestroyOnLoad(gameObject);
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        private void Start()
        {
            ChangeState(State.Loggin);
        }

        private void Update()
        {
            Workflow();
        }

        private void Workflow()
        {
            switch (current)
            {
                case State.Idle:
                    break;
                case State.Loggin:
                    {
                        SceneManager.LoadScene("Login");
                        MoveNext();
                    }
                    break;
                case State.LoadLoginData:
                    {
                        if (SettingsData.instance != null)
                        {
                            LoginUI.instance.Show();
                            MoveNext();
                        }
                    }
                    break;
                case State.WaitUntilLoggedIn:
                    {
                        if (User.isloggedIn)
                        {
                            LoginUI.instance.Hide();
                            MoveNext();
                        }
                    }
                    break;
                case State.LoadUserData:
                    {
                        if (AssetData.instance != null &&
                            InventoryData.instance != null &&
                            PlayerRoomsData.instance != null &&
                            QuestsPendingData.instance != null &&
                            QuestsAcceptedData.instance != null &&
                            QuestsInProgressData.instance != null &&
                            SettingsData.instance != null)
                        {
                            MoveNext();
                        }
                    }
                    break;
                case State.WaitUntilUserDataLoaded:
                    {
                        MoveNext();
                    }
                    break;
                case State.CheckPlayerHistory:
                    {
                        // todo -> 유저 실행 기록 체크 PlayerPref or Server
                        ChangeState(State.StartGame);
                    }
                    break;
                case State.DoFirstTutorial:
                    break;
                case State.WaitUntilFirstTutorialFinished:
                    break;
                case State.StartGame:
                    {
                        SceneManager.LoadScene("InGame");
                        MoveNext();
                    }
                    break;
                case State.LoadGameObjects:
                    {
                        if (SceneManager.GetActiveScene().name == "InGame" &&
                            AssetsLoader.instance != null)
                        {
                            AssetsLoader.instance.LoadAllAssets();
                            MoveNext();
                        }
                    }
                    break;
                case State.WaitUntilGameObjectsLoaded:
                    {
                        if (AssetsLoader.instance.isLoaded)
                        {
                            MoveNext();
                        }
                    }
                    break;
                case State.InitializeObjectPools:
                    {
                        if (InventoryUI.instance.gameObject.activeSelf == false &&
                            QuestBoxUI.instance.gameObject.activeSelf == false)
                        {
                            MoveNext();
                        }
                    }
                    break;
                case State.StartIntro:
                    {
                        IntroManager.instance.Play();
                        MoveNext();
                    }
                    break;
                case State.WaitUntilIntroFinished:
                    {
                        if (IntroManager.instance.isFinished)
                            MoveNext();
                    }
                    break;
                case State.DefaultSetUp:
                    {
                        if (ControllerManager.instance != null)
                        {
                            PlayerRoomUI.instance.Show();
                            QuestManager.instance.RunEventTree();
                            MoveNext();
                        }
                    }
                    break;
                case State.InGame:
                    {
                        // nothing to do
                    }
                    break;
                default:
                    break;
            }
        }

        private void MoveNext() => current++;
    }
}