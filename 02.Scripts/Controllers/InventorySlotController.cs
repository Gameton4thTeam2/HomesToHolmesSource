using UnityEngine;
using HTH.UI;
using HTH.DataModels;
using HTH.WorldElements;
using HTH.DataStructures;
using HTH.DataDependencySources;
using HTH.AudioSystems;
using System;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;

namespace HTH.InputHandlers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_12
    /// 설명    : 인벤토리의 슬롯의 아이템을 드래그해서 유저가 방에 가져다놓을 수 있게 하는 핸들러
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class InventorySlotController : MonoBehaviour, IController
    {
        public bool controllable { get; set; }

        private InventorySlot _slot;
        public Action<Item> OnSelected;
        public Action<Item> OnDeselected;
        private BoxCollider[] _cols;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        [SerializeField] private LayerMask _targetMask;
        private int _itemLayer;
        private int _groundLayer;
        private int _wallLayer;
        private Camera _worldCam;
        private RaycastHit[] _hits;
        private Ray _ray;
        private Sound _itemSound;
        private GridSnappingHelper _gridSnapping;
        [SerializeField] private BoxTriggersCaster _caster;
        private RaycastHit _hit;
        private ConcurrentQueue<Action> _createItemQueue = new ConcurrentQueue<Action>();
        private InventoryPresenter _inventoryPresenter;

        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Handle(InventorySlot slot)
        {
            if (ControllerManager.instance.HandOverControlTo(this))
            {
                GameObject original = ItemAssets.instance[slot.item.id].prefab;

                _slot = slot;
                _meshFilter.mesh = original.GetComponent<MeshFilter>().sharedMesh;
                _meshRenderer.materials = original.GetComponent<MeshRenderer>().sharedMaterials;

                // todo -> Pooling box colliders.
                _cols = original.GetComponents<BoxCollider>();
                ItemControllerHelper.instance.Active(transform);
            }
        }

        public void Cancel()
        {
            if (ControllerManager.instance.ReturnControl(this))
            {
                _slot = null;
                _meshFilter.mesh = null;
                _meshRenderer.material = null;
                transform.position = new Vector3(-5000.0f, -5000.0f, -5000.0f);
                ItemControllerHelper.instance.Deactive(null);
            }
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
            _inventoryPresenter = new InventoryPresenter();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _worldCam = CameraController.instance.worldCam;
            _itemSound = SFXAssets.GetSFX("Item");
            _itemLayer = LayerMask.NameToLayer("Item");
            _groundLayer = LayerMask.NameToLayer("Ground");
            _wallLayer = LayerMask.NameToLayer("Wall");
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => ControllerManager.instance != null);
                ControllerManager.instance.Register(this);

                await UniTask.WaitUntil(() => GridSnappingHelper.instance != null);
                _gridSnapping = GridSnappingHelper.instance;

                await UniTask.WaitUntil(() => BoxTriggersCaster.instance != null);
                _caster = BoxTriggersCaster.instance;
            });
        }

        private void Update()
        {
            if (controllable)
            {
                if (Input.GetMouseButton(0))
                {
                    Transform room = Player.instance.currentRoom.transform;
                    bool moveAvailable = false;

                    // 벽 배치 체크
                    _ray = _worldCam.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, 1 << _wallLayer) &&
                        (ItemAssets.instance[_slot.item.id].options & ItemInfo.OptionFlags.HangOnWall) > 0)
                    {
                        Vector3 pos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Y | Axis.Z);
                        Quaternion rot = _hit.collider.transform.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                        _caster
                            .CastAsync(_cols, pos, rot, _targetMask)
                            .OnCasted((result) =>
                            {
                                if (result == false)
                                {
                                    transform.position = pos;
                                    transform.rotation = rot;
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
                            Quaternion rot = room.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                            _caster
                                .CastAsync(_cols, pos, rot, _targetMask)
                                .OnCasted((result) =>
                                {
                                    if (result == false)
                                    {
                                        transform.position = pos;
                                        transform.rotation = rot;
                                    }

                                    ItemControllerHelper.instance.RefreshMaterial(result == false);
                                });
                            return;
                        }
                    };
                    return;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Transform room = Player.instance.currentRoom.transform;
                    bool moveAvailable = false;

                    // 벽 배치 체크
                    _ray = _worldCam.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, 1 << _wallLayer) &&
                        (ItemAssets.instance[_slot.item.id].options & ItemInfo.OptionFlags.HangOnWall) > 0)
                    {   
                        int id = _slot.item.id;
                        GameObject prefab = ItemAssets.instance[_slot.item.id].prefab;
                        Vector3 pos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Y | Axis.Z);
                        Quaternion rot = _hit.collider.transform.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                        transform.position = pos;
                        _createItemQueue.Enqueue(() => CreateCurrentItem(id, prefab, pos, rot, room));
                        _caster
                            .CastAsync(_cols, pos, rot, (1<<_itemLayer & 1<<_groundLayer))
                            .OnCasted((result) =>
                            {
                                if (result == false)
                                {
                                    _createItemQueue.Enqueue(() => CreateCurrentItem(id, prefab, pos, rot, room));
                                    Cancel();
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
                            int id = _slot.item.id;
                            GameObject prefab = ItemAssets.instance[_slot.item.id].prefab;
                            Vector3 pos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Z);
                            Quaternion rot = room.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                            transform.position = pos;
                            _caster
                                .CastAsync(_cols, pos, rot, _targetMask)
                                .OnCasted((result) =>
                                {
                                    if (result == false)
                                    {
                                        _createItemQueue.Enqueue(() => CreateCurrentItem(id, prefab, pos, rot, room));
                                        Cancel();
                                    }

                                    ItemControllerHelper.instance.RefreshMaterial(result == false);
                                });
                            return;
                        }
                    };

                    Cancel();
                }
            }

            // 아이템 생성 큐 실행
            Action createItemHandler;
            while (_createItemQueue.Count > 0)
            {
                if (_createItemQueue.TryDequeue(out createItemHandler))
                {
                    createItemHandler.Invoke();
                    _createItemQueue.Clear();
                }
            }
        }

        private void CreateCurrentItem(int itemID, GameObject prefab, Vector3 pos, Quaternion rotation, Transform parent)
        {
            if (_inventoryPresenter.removeCommand.TryExecute(new ItemPair(itemID, 1)))
            {
                Item item = Instantiate(prefab, pos, rotation, parent).GetComponent<Item>();
                AudioManager.instance.PlaySFX(_itemSound);
                Player.instance.currentRoom.AddItem(item);
                PlayerRoomsManager.instance.SaveRooms();
            }
        }
    }
}
