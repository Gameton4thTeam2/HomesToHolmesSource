using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using HTH.DataModels;
using HTH.DataStructures;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 인벤토리 UI 슬롯
    /// </summary>
    public class InventorySlot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler
    {
        private ItemPair _item;
        public ItemPair item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;

                if (value != ItemPair.empty)
                {
                    _icon.sprite = ItemAssets.instance[value.id].icon;
                    _num.text = value.num.ToString();
                    OnItemSet?.Invoke();
                }
                else
                {
                    _icon.sprite = null;
                    _num.text = string.Empty;
                    OnItemClear?.Invoke();
                }
            }
        }
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _num;
        public event Action OnItemSet;
        public event Action OnItemClear;
        private bool _isSelected;
        private Vector2 _dragBeginPoint;
        private float _draggingDistance = 0.02f;

        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Select()
        {
            InventoryUI.instance.SelectSlot(this);
            _isSelected = true;
        }

        public void Deselect()
        {
            _isSelected = false;
        }

        public void Increase()
        {
            _item.num++;
        }

        public void Decrease()
        {
            _item.num--;
            if (_item.num <= 0)
                item = ItemPair.empty;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isSelected == false)
                return;

            if (Vector2.Distance(_dragBeginPoint, eventData.position) > _draggingDistance)
                InventoryUI.instance.BeginSlotHandle(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isSelected == false)
                return;

            _dragBeginPoint = eventData.position;
        }
    }
}