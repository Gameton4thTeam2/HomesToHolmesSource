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
    /// </summary>
    public class ConfirmWindowPopUpUI : UIMonoBehaviour<ConfirmWindowPopUpUI>
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TMP_Text _content;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void Show(UnityAction onConfirm = null, UnityAction onCancel = null, string content = "CONFIRM_DEFAULT_1")
        {
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _confirmButton.enabled = onConfirm != null;
            if (onCancel == null)
                onCancel = Hide;
            _content.text = content;
            _confirmButton.onClick.AddListener(onConfirm);
            _cancelButton.onClick.AddListener(onCancel);
            base.Show();
        }

        public void Show(UnityAction onConfirm = null, string content = "CONFIRM_DEFAULT_1")
        {
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _confirmButton.enabled = onConfirm != null;            
            _content.text = content;
            _confirmButton.onClick.AddListener(onConfirm);
            _cancelButton.onClick.AddListener(Hide);
            base.Show();
        }

        new public void Hide()
        {
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            base.Hide();
        }
    }
}