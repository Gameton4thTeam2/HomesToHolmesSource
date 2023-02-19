using UnityEngine;
using HTH.UI;
using HTH.DataModels;
using HTH.WorldElements;
using HTH.GameSystems;
using HTH.AudioSystems;
using System.Collections.Concurrent;
using System;
using Cysharp.Threading.Tasks;
using HTH.DataDependencySources;
using HTH.DataStructures;

namespace HTH.InputHandlers
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class QuestInventorySlotController : MonoBehaviour, IController
    {
        public bool controllable { get; set; }
        private QuestInventorySlot _slot;
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
        private RaycastHit _hit;
        [SerializeField] private BoxTriggersCaster _caster;
        private ConcurrentQueue<Action> _createItemQueue = new ConcurrentQueue<Action>();


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Handle(QuestInventorySlot slot)
        {
            if (ControllerManager.instance.HandOverControlTo(this))
            {
                GameObject original = ItemAssets.instance[slot.id].prefab;

                _slot = slot;
                _meshFilter.mesh = original.GetComponent<MeshFilter>().sharedMesh;
                _meshRenderer.materials = original.GetComponent<MeshRenderer>().sharedMaterials;

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
                        (ItemAssets.instance[_slot.id].options & ItemInfo.OptionFlags.HangOnWall) > 0)
                    {
                        Vector3 pos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Y | Axis.Z);
                        Quaternion rot = room.rotation * _hit.collider.transform.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                        _caster
                            .CastAsync(_cols, pos, rot, _targetMask)
                            .OnCasted((result) =>
                            {
                                if (result == false)
                                {
                                    transform.position = pos;
                                    transform.rotation = _hit.collider.transform.rotation * Quaternion.Euler(Vector3.up * 180.0f);
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
                                .CastAsync(_cols, pos, rot, (1 << _groundLayer) | (1 << _itemLayer))
                                .OnCasted((result) =>
                                {
                                    if (result == false)
                                    {
                                        transform.position = pos;
                                        transform.rotation = room.rotation * Quaternion.Euler(Vector3.up * 180.0f);
                                    }

                                    ItemControllerHelper.instance.RefreshMaterial(result == false);
                                });
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
                        (ItemAssets.instance[_slot.id].options & ItemInfo.OptionFlags.HangOnWall) > 0)
                    {
                        int id = _slot.id;
                        GameObject prefab = ItemAssets.instance[_slot.id].prefab;
                        Vector3 pos = _gridSnapping.GetAdjustedPosToGrid(_hit.point, Axis.X | Axis.Y | Axis.Z);
                        Quaternion rot = room.rotation * _hit.collider.transform.rotation * Quaternion.Euler(Vector3.up * 180.0f);
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
                            int id = _slot.id;
                            GameObject prefab = ItemAssets.instance[_slot.id].prefab;
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
            Item item = Instantiate(prefab, pos, rotation, parent).GetComponent<Item>();
            AudioManager.instance.PlaySFX(_itemSound);
            QuestManager.instance.items.Add(item);
        }
    }
}
