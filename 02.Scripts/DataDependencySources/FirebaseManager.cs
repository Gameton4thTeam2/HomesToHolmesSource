using UnityEngine;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase;
using Google;
using Firebase.Database;
using HTH.UI;
using HTH.DataModels;
using HTH.GameSystems;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자      : 권병석
    /// 작성일      : 2023_01_26
    /// 설명        : 파이어베이스 사용을 위한 매니저 구현
    /// 기존 로그인 정보가 있지만 닉네임을 등록하지 않았을경우 입력창이 뜨게 수정, 인트로 시청 여부 추가 [2023_02_01, 권병석]
    /// 데이터베이스 경로를 반환하는 인덱서 추가, 출석관련 함수 추가
    /// 
    /// 수정자  : 조영민
    /// 수정일  : 2023_02_11
    /// 수정내용 : 인트로 시청 여부에대해 물어보고 결과를 콜백받기위한 체이닝함수로 수정
    /// </summary>
    public class FirebaseManager : MonoBehaviour
    {
        /// <summary>
        /// 설명 : 구글 로그인 할 때 파이어베이스에 저장하는 User 데이터 클래스
        /// </summary>
        public class User
        {
            public string userName;
            public string email;
            public User(string userName, string email)
            {
                this.userName = userName;
                this.email = email;
            }
        }

        public static FirebaseManager instance
        {
            get
            {
                return _instance;
            }
            set
            {
                if (_instance == null)
                {
                    _instance = value;
                }
            }
        }
        private static FirebaseManager _instance;

        private string _googleWebAPI = "562195074413-q4me6n7v7si9f1llvv7sr93nbmvotsvl.apps.googleusercontent.com";
        private GoogleSignInConfiguration _configuration;
        private FirebaseAuth _auth;
        private FirebaseUser _user;
        private DatabaseReference _reference;
        private DatabaseReference _itemDataPath;
        private DatabaseReference _userPath;
        public DateTime _serverTime;
        private bool _introHasPlayed;
        private event Action<bool> _onGetIntroHistoryComplete;

        //===========================================================================
        //                             Public Methods
        //===========================================================================

        /// <summary>
        /// 데이터베이스 경로를 반환하는 인덱서
        /// </summary>
        /// <param name="kindOfPath"> 0 : 루트 노드, 1 : 유저 정보, 2 : 유저 게임 데이터</param>
        /// <param name="key">불어올 데이터 키 값</param>
        /// <returns></returns>
        public DatabaseReference this[int kindOfPath, string key]
        {
            get
            {
                try
                {
                    if (kindOfPath == 0)
                        return _reference.Child(key);
                    else if (kindOfPath == 1)
                        return _userPath.Child(key);
                    else if (kindOfPath == 2) // todo => 유저 게임 정보를 데이터베이스에 저장시킬때 사용
                        return _itemDataPath.Child(key);
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public void OnClickGoogleLoginButton()
        {
            GoogleSignIn.Configuration = _configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;
            // 구글 로그인 시작
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnGoogleAuthenticatedFinished);
        }

        public void MakeNickNameDirectory(string name)
        {
            Dictionary<string, object> dictData = new Dictionary<string, object>();
            dictData.Add("NickName", name);
            _reference.Child(_user.UserId).UpdateChildrenAsync(dictData);
        }

        public void MakeIntroDirectory()
        {
            Dictionary<string, object> dictData = new Dictionary<string, object>();
            dictData.Add("Intro", true);
            _reference.Child(_user.UserId).UpdateChildrenAsync(dictData);
        }

        /// <summary>
        /// 유저가 인트로 진행했는지 결과를 통지받고싶으면 OnIntroHistoryCompleted 로 콜백함수 등록해야함.
        /// </summary>
        public FirebaseManager GetIntroHistoryAsync()
        {
            _reference.Child(_user.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if(task.IsFaulted == false &&
                   task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        IDictionary<string, object> item = (Dictionary<string, object>)snapshot.Value;
                        if(item.ContainsKey("Intro"))
                        {
                            _introHasPlayed = true;
                        }
                        else
                        {
                            _introHasPlayed = false;
                            MakeIntroDirectory();
                        }
                        _onGetIntroHistoryComplete?.Invoke(_introHasPlayed);
                    }
                }
            });
            return this;
        }

        /// <summary>
        /// 인트로 이력 결과 통지용
        /// </summary>
        public FirebaseManager OnGetIntroHistoryComplete(Action<bool> onComplete)
        {
            _onGetIntroHistoryComplete = onComplete;
            return this;
        }

        /// <summary>
        /// 서버 시간을 리프레쉬하는 함수
        /// Unix Time, MilliSecond 형식, 한국 시간 기준으로 저장함
        /// </summary>
        public async void RefreshServerTime()
        {
            await _reference.Child("ServerTime").SetValueAsync(ServerValue.Timestamp);
            var serverTimeSnapshot = await _reference.Child("ServerTime").GetValueAsync();
            double timeStamp = double.Parse(serverTimeSnapshot.Value.ToString());
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0); // Unix Time 계산 위한 변수
            _serverTime = dateTime.AddMilliseconds(timeStamp).AddHours(9f); // Korean Time으로 계산
            TimeSpan timeSpan = _serverTime - dateTime;
            await _reference.Child("ServerTime").SetValueAsync(timeSpan.TotalMilliseconds); // Unit Time으로 저장
            Debug.Log($"[FirebaseManager] : Refresh Server Time {_serverTime}");
        }
        
        /// <summary>
        /// 출석정보를 데이터베이스에 등록 및 갱신하는 함수
        /// </summary>
        /// <param name="grid">아이템 그리드 크기</param>
        /// <param name="count">선택된 그리드 개수</param>
        /// <param name="index">아이템 인덱스</param>
        /// <param name="selectedGrid">선택된 그리드 정보</param>
        public async void RegisterAndInitAttendanceOnDatabase(int grid, int count, int index, int[] selectedGrid)
        {
            await _userPath.Child("AttendanceItemGrid").SetValueAsync(grid);
            await _userPath.Child("AttendanceItemCount").SetValueAsync(count);
            await _userPath.Child("AttendanceItemGridSelected").SetValueAsync(selectedGrid);
            await _userPath.Child("AttendanceItemIndex").SetValueAsync(index);
        }

        /// <summary>
        /// Preview 데이터를 데이터베이스에 저장하는 함수
        /// </summary>
        /// <param name="grid">아이템 그리드 크기</param>
        /// <param name="index">아이템 인덱스</param>
        /// <param name="changeCount">랜덤 교체 횟수</param>
        public async void SavePreviewAttendanceDataOnDatabase(int grid, int index, int changeCount)
        {
            await _userPath.Child("AttendancePreviewItemGrid").SetValueAsync(grid);
            await _userPath.Child("AttendancePreviewItemIndex").SetValueAsync(index);
            await _userPath.Child("AttendancePreviewChangeCount").SetValueAsync(changeCount);
        }

        /// <summary>
        /// 사용자가 마지막으로 출석한 정보를 데이터베이스에 저장하는 함수
        /// </summary>
        public async void SaveAttendanceTimeFromDatabase()
        {
            await _userPath.Child("AttendanceLastTime").SetValueAsync(ServerValue.Timestamp);
            var lastTimeSnapshot = await _userPath.Child("AttendanceLastTime").GetValueAsync();
            double timeStamp = double.Parse(lastTimeSnapshot.Value.ToString());
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime attendanceTime = dateTime.AddMilliseconds(timeStamp).AddHours(9f);
            TimeSpan timeSpan = attendanceTime - dateTime;
            Debug.Log($"[FirebaseManager] : Init Attendance Time {attendanceTime}, {timeSpan.TotalMilliseconds}");
            await _userPath.Child("AttendanceLastTime").SetValueAsync(timeSpan.TotalMilliseconds);
        }

        /// <summary>
        /// 선택한 그리드 정보를 데이터베이스에 저장하는 함수
        /// </summary>
        /// <param name="count">선택된 그리드 개수</param>
        /// <param name="selectedGrid">선택한 그리드 정보</param>
        public async void SaveSelectedItemGridFromDatabase(int count, int[] selectedGrid)
        {
            await _userPath.Child("AttendanceItemCount").SetValueAsync(count);
            await _userPath.Child("AttendanceItemGridSelected").SetValueAsync(selectedGrid);
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            instance = this;
            _configuration = new GoogleSignInConfiguration
            {
                WebClientId = _googleWebAPI,
                RequestIdToken = true
            };
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            // 파이어베이스 DependencyStatus 체크
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitFirebase();
                    InitDatabase();
                }
                else
                {
                    Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
            
        }

        private void InitFirebase()
        {
            _auth = FirebaseAuth.DefaultInstance;
        }

        private void InitDatabase()
        {
            _reference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        private void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Fault");
                return;
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("Login Cancel");
                return;
            }
            else
            {
                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                // 로그인 시작
                _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithCredentialAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithCredentialAsync encountered an error : " + task.Exception);
                        return;
                    }

                    _user = _auth.CurrentUser; // 현재 로그인 유저 정보 저장
                    _itemDataPath = _reference.Child(_user.UserId).Child("Assets"); // 데이터 저장 Path
                    _userPath = _reference.Child(_user.UserId); // 유저 데이터 저장 Path

                    // 데이터베이스에 UserId로 만들어진 데이터가 있는지 확인하는 부분
                    _reference.Child(_user.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted)
                            return;
                        else if (task.IsCompleted)
                        {
                            DataSnapshot snapshot = task.Result;
                            if (snapshot.Exists) // 데이터가 존재한다면
                            { // 닉네임을 불러와서 예전에 입력했던 닉네임을 불러와 로그인하는 패널을 띄움
                                IDictionary<string, object> item = (IDictionary<string, object>)snapshot.Value;
                                if (item.ContainsKey("NickName"))
                                { // 저장된 닉네임이 있을때
                                    LoginUI.instance.IsAlreadyLogin((string)item["NickName"]);
                                    SettingsData.instance.hasGoogleLoginData = 1;
                                }
                                else
                                { // 저장된 닉네임이 없을때
                                    LoginUI.instance.SuccessGoogleLogin();
                                    SettingsData.instance.hasGoogleLoginData = 1;
                                }
                            }
                            else // 존재하지 않으면
                            { // 데이터베이스에 유저 정보를 입력하고 닉네임을 입력해서 로그인하는 패널을 띄움
                                SaveOnDatabase(_user.UserId, _user.DisplayName, _user.Email);
                                LoginUI.instance.SuccessGoogleLogin();
                                SettingsData.instance.hasGoogleLoginData = 1;
                            }
                        }
                    });
                });
            }
        }

        /// <summary>
        /// 설명 : 초기 데이터베이스에 유저 정보를 저장하는 함수
        /// </summary>
        private void SaveOnDatabase(string userId, string name, string email)
        {
            User user = new User(name, email);

            string json = JsonUtility.ToJson(user);

            _reference.Child(userId).SetRawJsonValueAsync(json);
        }
    }
}