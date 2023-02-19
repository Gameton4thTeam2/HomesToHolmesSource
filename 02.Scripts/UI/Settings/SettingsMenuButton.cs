using UnityEngine;
using UnityEngine.Events;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_12
    /// 설명    : 세팅 UI의 메뉴 버튼 / 메뉴 판넬의 자식 버튼들에게 부여
    /// </summary>
    public class SettingsMenuButton : MonoBehaviour
    {
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeSelected;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Select() // 메뉴 버튼을 선택하면 실행 됨
        {
            if (onTabSelected != null)
                onTabSelected.Invoke();
        }

        public void Deselect() // 다른 메뉴 버튼을 선택하면 실행 됨
        {
            if (onTabDeSelected != null)
                onTabDeSelected.Invoke();
        }

        public void OnSelectButton(SettingsMenuButton button) // 해당 메뉴 버튼을 선택한 것을 메뉴 컨트롤러에게 알려줌
        {
            SettingsMenuController.instance.SelectedButton(button);
        }
    }
}