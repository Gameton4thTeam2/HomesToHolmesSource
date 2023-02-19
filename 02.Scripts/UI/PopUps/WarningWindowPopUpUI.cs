using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 확인창
    /// 수정자  : 권병석
    /// 수정자  : 2023_02_09
    /// 설명    : 오류 수정 및 제목과 메세지를 가지는 Show함수 추가
    /// </summary>
    public class WarningWindowPopUpUI : UIMonoBehaviour<WarningWindowPopUpUI>
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _message;
        [SerializeField] private TMP_Text _title;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void Show(string message, UnityAction onClose = null)
        {
            _closeButton.onClick.RemoveAllListeners();

            _message.text = message;
            if (onClose == null)
                onClose = Hide;
            _closeButton.onClick.AddListener(onClose);
            base.Show();
        }

        public void Show(string title, string message, UnityAction onClose = null)
        {
            _closeButton.onClick.RemoveAllListeners();
            _title.text = title;
            _message.text = message;
            if (onClose == null)
                onClose = Hide;
            _closeButton.onClick.AddListener(onClose);
            base.Show();
        }

        public void Show(string message, float delayToHide, UnityAction onClose = null)
        {
            _closeButton.onClick.RemoveAllListeners();
            _message.text = message;
            if (onClose == null)
                onClose = Hide;
            _closeButton.onClick.AddListener(onClose);
            base.Show();
            Invoke("Hide", delayToHide);
        }

        new public void Hide()
        {
            _closeButton.onClick.RemoveAllListeners();
            base.Hide();
        }
    }
}