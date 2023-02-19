using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTH.DataModels;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 상점 ( 유저 귀속 )
    /// </summary>
    public class ItemShopUI : UIMonoBehaviour<ItemShopUI>
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private List<ItemInfo> _itemList;
        [SerializeField] private ItemShopSlot _slotPrefab;
        private List<ItemShopSlot> _slots;
        public IEnumerable<ItemShopSlot> GetSlots() => _slots;


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override protected void Init()
        {
            _slots = new List<ItemShopSlot>();
            ItemShopSlot slot;
            foreach (ItemInfo itemInfo in _itemList)
            {
                slot = Instantiate(_slotPrefab, _content);
                slot.itemInfo = itemInfo;
                _slots.Add(slot);
            }
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, (_itemList.Count / 4) * 300);
            HideUnmanaged();
        }
    }
}