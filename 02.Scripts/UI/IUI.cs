using System;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : UI 컴포넌트 기본 추상멤버들
    /// </summary>
    public interface IUI
    {
        int sortingOrder { set; }

        event Action OnShow;
        event Action OnHide;

        void Show();
        void Hide();
        void ShowUnmanaged();
        void HideUnmanaged();
        void ShowWithoutNotification();
        void HideWithoutNotification();
    }
}