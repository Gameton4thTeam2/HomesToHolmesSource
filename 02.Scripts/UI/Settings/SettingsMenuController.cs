using UnityEngine;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_12
    /// 설명    : 세팅 UI의 메뉴 컨트롤러 / 메뉴 버튼들을 자식으로 가지는 판넬에 부여
    /// </summary>
    public class SettingsMenuController : MonoBehaviour
    {
        public static SettingsMenuController instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<SettingsMenuController>();
                }
                return _instance;
            }
        }
        private static SettingsMenuController _instance;
        SettingsMenuButton menuButton;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void SelectedButton(SettingsMenuButton button) // MenuButton으로 부터 지금 저장된 버튼과 다른 버튼이 선택됐다는 알림을 받으면
        {
            if (menuButton != null)
                menuButton.Deselect(); // Deselect화 시키고

            menuButton = button;
            menuButton.Select(); // Select화 시키기
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Start()
        {
            SelectedButton(transform.GetChild(0).GetComponent<SettingsMenuButton>());
        }
    }
}