using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_02_09
    /// 설명    : 보상 창
    /// </summary>
    public class RewardWindowPopUpUI : UIMonoBehaviour<RewardWindowPopUpUI>
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _content;
        [SerializeField] private TMP_Text _countTxt;
        [SerializeField] private Image _image;
        [SerializeField] private Image _ribonImage;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void Show(UnityAction onConfirm = null, string content = "REWARD_DEFAULT_1", Image image = null, Color ribonColor = new Color(), int num = 0)
        {
            _confirmButton.onClick.RemoveAllListeners();

            _confirmButton.enabled = onConfirm != null;
            _content.text = content;
            _countTxt.text = num.ToString();
            _image.color = image.color;
            _image.sprite = image.sprite;
            _ribonImage.color = ribonColor;
            _confirmButton.onClick.AddListener(onConfirm);
            base.Show();
        }
        
        new public void Hide()
        {
            _confirmButton.onClick.RemoveAllListeners();
            base.Hide();
        }
    }
}