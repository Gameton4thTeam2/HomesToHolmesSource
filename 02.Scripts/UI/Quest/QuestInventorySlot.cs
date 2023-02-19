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
    /// 작성일  : 2023_01_20
    /// 설명    : 퀘스트 내 상점용 임시 인벤토리 UI 슬롯
    /// </summary>
    public class QuestInventorySlot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler
    {
        private int _id;
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;

                if (ItemAssets.instance[value] == null)
                {
                    gameObject.SetActive(false);
                    _id = -1;
                    Deselect();
                }
                _icon.sprite = ItemAssets.instance[value].icon;
            }
        }
        [SerializeField] private Image _icon;
        private bool _isSelected;
        private float _draggingDistance = 0.02f;
        private Vector2 _dragBeginPoint;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Select()
        {
            QuestRoomUI.instance.SelectSlot(this);
            _isSelected = true;
        }

        public void Deselect()
        {
            _isSelected = false;
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
                QuestRoomUI.instance.BeginSlotHandle(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isSelected == false)
                return; 

            _dragBeginPoint = eventData.position;
        }
    }
}