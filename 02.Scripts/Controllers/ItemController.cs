using System;
using HTH.WorldElements;
using HTH.UI;
using HTH.UnityAPIWrappers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HTH.DataModels;
using static UnityEngine.UI.Image;
using Unity.Entities.UniversalDelegates;
using HTH.Tools;
using Cysharp.Threading.Tasks;

namespace HTH.InputHandlers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 유저가 월드의 아이템을 조작하기위한 클래스
    /// </summary>
    public class ItemController : MonoBehaviour, IController
    {
        public static ItemController instance;
        public bool controllable
        {
            get => enabled;
            set => enabled = value;
        }
        private Item _selected;
        public Item selected
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
                    _boxCollidersCenterBuffer = new Vector3[_cols.Length];
                    _boxCollidersSizeBuffer = new Vector3[_cols.Length];
                    for (int i = 0; i < _cols.Length; i++)
                    {
                        _boxCollidersCenterBuffer[i] = _cols[i].center;
                        _boxCollidersSizeBuffer[i] = _cols[i].size;
                    }
                    OnSelected?.Invoke(value);
                }
                else
                {
                    _boxCollidersCenterBuffer = null;
                    _boxCollidersSizeBuffer = null;
                    OnDeselected?.Invoke(value);
                }

                _selected = value;
            }
        }
        private BoxCollider[] _cols;
        private Vector3[] _boxCollidersCenterBuffer;
        private Vector3[] _boxCollidersSizeBuffer;
        public Action<Item> OnSelected;
        public Action<Item> OnDeselected;
        public Vector2 mouseDownPos;
        public Vector2 mouseUpPos;
        public Vector2 mousePrevPos;
        public float dragDeltaMin = 10.0f;
        public bool isSelected;
        public bool isPicking
        {
            get
            {
                return _isPicking;
            }
            set
            {
                if (_isPicking == value)
                    return;

                _isPicking = value;
                if (value)
                    OnBeginPick?.Invoke();
                else
                    OnEndPick?.Invoke();
            }
        }
        private bool _isPicking;
        public event Action OnBeginPick;
        public event Action OnEndPick;
        public bool isDragging;
        public float dragSpeed = 0.1f;
        private Camera _worldCam;
        private RaycastHit[] _hits;
        RaycastHit _hit;
        private Ray _ray;
        [SerializeField] private LayerMask _targetMask;
        private int _itemLayer;
        private int _itemSelectedLayer;
        private int _groundLayer;
        private int _wallLayer;
        [SerializeField] private CustomStandaloneInputModule _standaloneInputModule;
        private BoxTriggersCaster _caster;
        private GridSnappingHelper _gridSnapping;
        [SerializeField] private CameraController _cameraController;
        private Vector3 _posMem;
        [SerializeField] private float _senstivity = 0.001f;
        [SerializeField] private float _dragDistance = 0.01f;
        [SerializeField] private EventSystem _eventSystem;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Select(Item item)
        {
            if (selected != null)
                _selected.gameObject.layer = _itemLayer;

            if (item != null)
                item.gameObject.layer = _itemSelectedLayer;

            selected = item;
            isSelected = item;

            if (selected != null)
            {
                ItemUI.instance.ShowUnmanaged(item);
                _gridSnapping.SetTarget(item.transform);
               
            }
            else
            {
                ItemUI.instance.HideUnmanaged();
                _gridSnapping.SetTarget(null);
            }
        }

        public void Pick()
        {
            RequestControl();
            isPicking = true;
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

        private void Awake()
        {
            instance = this;
            _worldCam = CameraController.instance.worldCam;
            _itemLayer = LayerMask.NameToLayer("Item");
            _itemSelectedLayer = LayerMask.NameToLayer("ItemHandling");
            _groundLayer = LayerMask.NameToLayer("Ground");
            _wallLayer = LayerMask.NameToLayer("Wall");

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => ItemControllerHelper.instance != null);

                OnSelected += (item) => ItemControllerHelper.instance.Active(item.transform);
                OnDeselected += (item) => ItemControllerHelper.instance.Deactive(null);

                await UniTask.WaitUntil(() => ItemUI.instance != null);
                                
                OnBeginPick += () => ItemUI.instance.SetTransparency(0.0f);
                OnEndPick += () => ItemUI.instance.SetTransparency(1.0f);

                await UniTask.WaitUntil(() => GridSnappingHelper.instance != null);

                _gridSnapping = GridSnappingHelper.instance;

                await UniTask.WaitUntil(() => BoxTriggersCaster.instance != null);

                _caster = BoxTriggersCaster.instance;
            });
        }

        private void Start()
        {
            ControllerManager.instance.RegisterAsDefault(this);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (_standaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(1<<LayerMask.NameToLayer("CastIgnoringUI"),
                                                                                 StandaloneInputModule.kMouseLeftId,
                                                                                 false))
            {
                return;
            }
#elif UNITY_ANDROID
            if (Input.touchCount > 0 &&
                _standaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(1 << LayerMask.NameToLayer("CastIgnoringUI"),
                                                                                 Input.GetTouch(0).fingerId,
                                                                                 false))
            {
                return;
            }
#endif

            if (isSelected &&
                selected == null)
            {
                Select(null);
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPos = Input.mousePosition;

                _ray = _worldCam.ScreenPointToRay(mouseDownPos);

                // 선택된 것 외에 다른것을 선택하려하면 기존 선택 취소
                if (selected)
                {
                    _posMem = _selected.transform.position;

                    // 선택된거 말고 다른걸 선택하려하면 기존 선택 취소시킴
                    if (Physics.Raycast(_ray, Mathf.Infinity, 1 << _itemSelectedLayer) == false)
                        Select(null);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseUpPos = Input.mousePosition;
                _ray = _worldCam.ScreenPointToRay(mouseUpPos);
                
                // 클릭 판정
                if (Vector2.Distance(mouseDownPos, mouseUpPos) < dragDeltaMin)
                {
                    // 아이템 선택
                    _hits = Physics.RaycastAll(_ray, Mathf.Infinity, 1 << _itemLayer);
                    if (_hits.Length > 0)
                    {
                        Select(_hits[0].collider.GetComponent<Item>());
                    }
                }

                isPicking = false;
            }
            else if (Input.GetMouseButton(0))
            {
                if (isPicking)
                {
                    Vector3 delta = _worldCam.transform.rotation * new Vector3(Input.mousePosition.x - mouseDownPos.x, 0.0f, Input.mousePosition.y - mouseDownPos.y);
                    delta = _gridSnapping.GetAdjustedPosToGrid(delta, Axis.X | Axis.Z) * _senstivity;
                
                    if (Vector3.Distance(_posMem, _posMem + delta) > _dragDistance)
                    {
                        Picking();
                    }
                }
                else
                {
                    mouseUpPos = Input.mousePosition;
                    _ray = _worldCam.ScreenPointToRay(mouseUpPos);
                    _hits = Physics.RaycastAll(_ray, Mathf.Infinity, 1 << _itemLayer);
                    if (_hits.Length <= 0)
                    {
                        ControllerManager.instance.HandOverControlFromTo(this, CameraController.instance);
                    }
                }
            }
            else
            {
            }
        }

        private void Picking()
        {
            bool moveAvailable = false;
            Transform room = Player.instance.currentRoom.transform;


            // 벽 배치 체크
            _ray = _worldCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, 1 << _wallLayer) &&
                (ItemAssets.instance[selected.id.value].options & ItemInfo.OptionFlags.HangOnWall) > 0)
            {
                Vector3 pos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Y | Axis.Z);
                Quaternion rot = room.rotation * _selected.transform.localRotation;
                _caster
                    .CastAsync(_cols, pos, rot, _targetMask)
                    .OnCasted((result) =>
                    {
                        if (result == false)
                        {
                            _selected.transform.position = pos;
                            _selected.transform.rotation = _hit.collider.transform.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                            _posMem = pos;
                            PlayerRoomsManager.instance.SaveRooms();
                        }

                        ItemControllerHelper.instance.RefreshMaterial(result == false);
                    });
                return;
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
                    Vector3 pos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Z);
                    Quaternion rot = room.rotation * _selected.transform.localRotation;
                    _caster
                        .CastAsync(_cols, pos, rot, _targetMask)
                        .OnCasted((result) =>
                        {
                            if (result == false)
                            {
                                _selected.transform.position = pos;
                                _posMem = pos;
                                PlayerRoomsManager.instance.SaveRooms();
                            }

                            ItemControllerHelper.instance.RefreshMaterial(result == false);
                        });
                }
            };
            return;
        }
    }
}