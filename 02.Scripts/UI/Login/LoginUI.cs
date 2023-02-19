using UnityEngine;
using TMPro;
using HTH.GameSystems;
using UnityEngine.UI;
using HTH.DataDependencySources;
using HTH.DataModels;
using Cysharp.Threading.Tasks;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_26
    /// 설명    : 로그인 UI 구현
    /// 수정자  : 권병석
    /// 수정일  : 2023_02_01
    /// 설명    : LoginUI가 매니저에 Register되지 않도록 수정
    /// </summary>
    public class LoginUI : UIMonoBehaviour<LoginUI>
    {
        protected override UIManager manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = UIManager.instance;
                }
                return _manager;
            }
        }

        [SerializeField] private TMP_InputField _userName;
        [SerializeField] private TMP_Text _nickName;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private TMP_Text _info2;
        [SerializeField] private Button _googleLoginButton;
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private Button _gameStartButton2;
        [SerializeField] private Button _openSettings;
        [SerializeField] private Button _guestLoginButton;
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _googleLoginPanel;
        [SerializeField] private GameObject _isAlreadyLoginPanel;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Login()
        {
            if(_loginPanel.activeSelf)
            {
                if (string.IsNullOrEmpty(_userName.text))
                    return;

                User.Login(_userName.text);
            }
            if (_isAlreadyLoginPanel.activeSelf)
            {
                if (string.IsNullOrEmpty(_nickName.text))
                    return;

                User.Login(_nickName.text);
            }
        }

        public void GuestLogin()
        {
            User.Login("Guest");
        }


        public void SuccessGoogleLogin()
        {
            _googleLoginPanel.SetActive(false);
            _guestLoginButton.gameObject.SetActive(false);
            _loginPanel.SetActive(true);
        }

        public void IsAlreadyLogin(string nickName)
        {
            _googleLoginPanel.SetActive(false);
            _guestLoginButton.gameObject.SetActive(false);
            _nickName.text = nickName;
            _isAlreadyLoginPanel.SetActive(true);
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        protected override void Init()
        {
            _googleLoginButton.onClick.AddListener(FirebaseManager.instance.OnClickGoogleLoginButton);
            _gameStartButton.onClick.AddListener(Login);
            _gameStartButton2.onClick.AddListener(Login);
            _guestLoginButton.onClick.AddListener(GuestLogin);
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => SettingsUI.instance != null);
                _openSettings.onClick.AddListener(SettingsUI.instance.Show);
            });

            if (SettingsData.instance.hasGoogleLoginData == 1)
            { // 구글 로그인을 한적이 있는지 PlayerPrefs 데이터에서 불러와서 체크 한 후 자동 로그인
                _googleLoginPanel.SetActive(false);
                FirebaseManager.instance.OnClickGoogleLoginButton();
            }
            base.Init();
        }

        private void Update()
        { // Text가 깜빡이는 효과를 주는 부분
            _info.alpha = Mathf.Abs(Mathf.Sin(Time.time * 3.0f));
            _info2.alpha = Mathf.Abs(Mathf.Sin(Time.time * 3.0f));
        }
    }
}
