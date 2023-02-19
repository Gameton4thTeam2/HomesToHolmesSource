using System.Collections.Generic;
using UnityEngine;
using HTH.DataDependencySources;
using HTH.DataStructures;
using HTH.Tools;
using HTH.InputHandlers;
using HTH.DataModels;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 인벤토리 UI
    /// </summary>
    public class InventoryUI : UIMonoBehaviour<InventoryUI>
    {
        private InventoryPresenter _presenter;
        [SerializeField] private RectTransform _content;
        [SerializeField] private InventorySlot _slotPrefab;
        private SimpleGameObjectPool<InventorySlot> _pool;
        [SerializeField] private InventorySlotController _inventorySlotHandler;
        private InventorySlot _slotSelected;
        [SerializeField] private GameObject _selectedSlotMark;
        [SerializeField] private MeshRenderer _previewMeshRenderer;
        [SerializeField] private MeshFilter _previewMeshFilter;
        [SerializeField] private Button _openInfo;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void BeginSlotHandle(InventorySlot slot)
        {
            _inventorySlotHandler.Handle(slot);
        }

        public void SelectSlot(InventorySlot slot)
        {
            if (_slotSelected != null)
                _slotSelected.Deselect();
            _slotSelected = slot;
            _selectedSlotMark.transform.SetParent(slot.transform);
            _selectedSlotMark.transform.localPosition = Vector2.zero;
            _selectedSlotMark.SetActive(true);
            RefreshPreviewModel(slot.item.id);
        }

        /// <summary>
        /// 아이템 모델 미리보기 
        /// </summary>
        public void RefreshPreviewModel(int itemID)
        {
            if (ItemAssets.instance[itemID])
            {
                _previewMeshRenderer.materials = ItemAssets.instance[itemID].prefab.GetComponent<MeshRenderer>().sharedMaterials;
                _previewMeshFilter.sharedMesh = ItemAssets.instance[itemID].prefab.GetComponent<MeshFilter>().sharedMesh;
            }
            else
            {
                _previewMeshRenderer.materials = null;
                _previewMeshFilter.sharedMesh = null;
            }
        }

        public override void Show()
        {
            SetSlots(_presenter.source);
            base.Show();
        }

        public override void ShowUnmanaged()
        {
            SetSlots(_presenter.source);
            base.ShowUnmanaged();
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override protected void Init()
        {
            _pool = new SimpleGameObjectPool<InventorySlot>(_slotPrefab, _content);
            _presenter = new InventoryPresenter();
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _presenter.source != null);
                _presenter.source.CollectionChanged += () =>
                {
                    SetSlots(_presenter.source);
                    _selectedSlotMark.gameObject.SetActive(false);
                };

                await UniTask.WaitUntil(() => ItemInfoUI.instance != null);
                _openInfo.onClick.AddListener(() =>
                {
                    if (_slotSelected != null)
                        ItemInfoUI.instance.Show(ItemAssets.instance[_slotSelected.item.id], Vector2.zero);
                });
            });

            base.Init();
        }

        /// <summary>
        /// 슬롯 전체 초기화 후 다시 세팅. 
        /// </summary>
        private void SetSlots(ICollection<ItemPair> items)
        {
            using (IEnumerator<ItemPair> e1 = items.GetEnumerator())
            using (IEnumerator<InventorySlot> e2 = _pool.Refresh(items.Count).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.item = e1.Current;
                }
            }

            Vector2 slotSize = _slotPrefab.GetComponent<RectTransform>().rect.size;
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, (items.Count + 1) * slotSize.y / 3);
        }
    }
}