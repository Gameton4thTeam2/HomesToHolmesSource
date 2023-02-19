using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 상점 장바구니 추가 UI
    /// </summary>
    public class ItemShopCartPurchasePopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text _totalPrice;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Button _cancelButton;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public event UnityAction OnPurchaseButtonClick
        {
            add
            {
                _purchaseButton.onClick.AddListener(value);
            }
            remove
            {
                _purchaseButton.onClick.RemoveListener(value);
            }
        }

        public event UnityAction OnCancelButtonClick
        {
            add
            {
                _cancelButton.onClick.AddListener(value);
            }
            remove
            {
                _cancelButton.onClick.RemoveListener(value);
            }
        }

        public void SetTotalPriceText(string text)
        {
            _totalPrice.text = text;
        }
    }
}