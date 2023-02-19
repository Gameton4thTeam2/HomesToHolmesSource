using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HTH.DataModels;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 상점 슬롯 UI
    /// </summary>
    public class ItemShopSlot : MonoBehaviour, IPointerClickHandler
    {
        private ItemInfo _itemInfo;
        public ItemInfo itemInfo
        {
            get
            {
                return _itemInfo;
            }
            set
            {
                _itemInfo = value;
                _icon.sprite = value.icon;
                _price.text = _itemInfo.buyPrice.GetSimplifiedString();
            }
        }
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _price;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void OnPointerClick(PointerEventData eventData)
        {
            ItemInfoUI.instance.Show(itemInfo, transform.position);
        }
    }
}