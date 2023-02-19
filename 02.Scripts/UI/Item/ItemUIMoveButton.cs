using HTH.DataModels;
using HTH.InputHandlers;
using HTH.WorldElements;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_15
    /// 설명    : Item 
    /// </summary>
    public class ItemUIMoveButton : MonoBehaviour, IController, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public bool controllable { get; set; }
        private ItemController _itemController;
        private RaycastHit[] _hits;
        private RaycastHit _hit;
        private Ray _ray;
        private Camera _worldCam;
        [SerializeField] private LayerMask _targetMask;
        private int _itemLayer;
        private int _itemSelectedLayer;
        private int _groundLayer;
        private int _wallLayer;
        private BoxTriggersCaster _caster;
        private GridSnappingHelper _gridSnapping;
        [SerializeField] private List<GameObject> _objectToToggle;

        private Item selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (value)
                {
                    _cols = value.GetComponents<BoxCollider>();
                    _itemId = value.id.value;
                    foreach (var obj in _objectToToggle)
                        obj.SetActive(false);
                    OnSelected?.Invoke(value);
                }
                else
                {
                    _itemId = -1;
                    foreach (var obj in _objectToToggle)
                        obj.SetActive(true);
                    OnDeselected?.Invoke(_selected);
                }

                _selected = value;
            }
        }
        private Item _selected;
        public Action<Item> OnSelected;
        public Action<Item> OnDeselected;
        private int _itemId = -1;
        private BoxCollider[] _cols;
        [SerializeField] private float _senstivity = 0.001f;
        [SerializeField] private float _dragDistance = 0.01f;
        private Vector2 _downEventPos;
        private Vector3 _posMem;
        [SerializeField] private Vector3 _expectedPos;
        [SerializeField] private Vector3 _expectedPosMem;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void OnDrag(PointerEventData eventData)
        {
            if (_selected == null)
            {
                return;
            }

            Vector3 delta = _worldCam.transform.rotation * new Vector3(eventData.position.x - _downEventPos.x, 0.0f, eventData.position.y - _downEventPos.y);
            delta = _gridSnapping.GetAdjustedPosToGrid(delta, Axis.X | Axis.Z) * _senstivity;
            _expectedPos = _posMem + delta;

            if (Vector3.Distance(_expectedPos, _expectedPosMem) > _dragDistance)
            {
                Pick(eventData);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

            if (RequestControl())
            {
                _downEventPos = eventData.position;
                selected = _itemController.selected;
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            RequestReturn();
            selected = null;
        }

        public bool RequestControl()
        {
            return ControllerManager.instance.HandOverControlTo(this);
        }

        public bool RequestReturn()
        {
            return ControllerManager.instance.ReturnControl(this);
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Start()
        {
            _itemController = ItemController.instance;
            _worldCam = CameraController.instance.worldCam;
            _gridSnapping = GridSnappingHelper.instance;
            _caster = BoxTriggersCaster.instance;
            _itemLayer = LayerMask.NameToLayer("Item");
            _itemSelectedLayer = LayerMask.NameToLayer("ItemHandling");
            _groundLayer = LayerMask.NameToLayer("Ground");
            _wallLayer = LayerMask.NameToLayer("Wall");
            OnSelected += (item) => ItemControllerHelper.instance.Active(item.transform);
            OnDeselected += (item) => ItemControllerHelper.instance.Deactive(null);
            ControllerManager.instance.Register(this);
        }

        private void Pick(PointerEventData eventData)
        {
            Transform room = Player.instance.currentRoom.transform;

            bool moveAvailable = false;

            // 벽 배치 체크
            _ray = _worldCam.ScreenPointToRay((Vector3)eventData.position - transform.localPosition);

            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, 1 << _wallLayer))
            {
                moveAvailable = (ItemAssets.instance[_itemId].options & ItemInfo.OptionFlags.HangOnWall) > 0;

                if (moveAvailable)
                {
                    _expectedPos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Y | Axis.Z);

                    _caster
                        .CastAsync(_cols, _expectedPos, room.rotation * _selected.transform.localRotation, _targetMask)
                        .OnCasted((result) =>
                        {
                            if (result == false)
                            {
                                _selected.transform.position = _expectedPos;
                                _selected.transform.rotation = _hit.collider.transform.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                                _posMem = _expectedPos;
                                _expectedPosMem = _expectedPos;
                            }

                            ItemControllerHelper.instance.RefreshMaterial(result);
                        });
                    return;
                }
            }

            Item pointed;
            // 바닥 / 아이템 위 체크 - 터치 포인트 기준으로 움직이기
            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, (1 << _groundLayer) | (1 << _itemLayer)))
            {
                moveAvailable |= _hit.collider.gameObject.layer == _groundLayer;
                moveAvailable |= _hit.collider.gameObject.layer == _itemLayer &&
                                 _hit.collider.TryGetComponent(out pointed) &&
                                 (ItemAssets.instance[pointed.id.value].options & ItemInfo.OptionFlags.StackOther) > 0;

                if (moveAvailable)
                {
                    _expectedPos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Z);

                    _caster
                        .CastAsync(_cols, _expectedPos, room.rotation * _selected.transform.localRotation, _targetMask)
                        .OnCasted((result) =>
                        {
                            if (result == false)
                            {
                                _selected.transform.position = _expectedPos;
                                _posMem = _expectedPos;
                                _expectedPosMem = _expectedPos;
                            }

                            ItemControllerHelper.instance.RefreshMaterial(result);
                        });
                }
            };
            return;
        }
    }
}
