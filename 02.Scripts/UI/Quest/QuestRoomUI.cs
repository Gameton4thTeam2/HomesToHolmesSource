using Cysharp.Threading.Tasks;
using HTH.DataModels;
using HTH.GameSystems;
using HTH.IDs;
using HTH.InputHandlers;
using HTH.Tools;
using HTH.WorldElements;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 의뢰 수행시 방 UI
    /// </summary>
    public class QuestRoomUI : UIMonoBehaviour<QuestRoomUI>
    {
        public IEnumerable<int> hashtagIDs => _hashtagPairs
                                                .Where(x => x.Value > 0)
                                                .Select(x => x.Key);

        [SerializeField] private TMP_Text _budget;
        [SerializeField] private TMP_Text _calc;
        [SerializeField] private MeshRenderer _previewMeshRenderer;
        [SerializeField] private MeshFilter _previewMeshFilter;
        [SerializeField] private RectTransform _itemsContent;

        // Inventory
        [SerializeField] private QuestInventorySlot _slotPrefab;
        public SimpleGameObjectPool<QuestInventorySlot> slotPool;
        [SerializeField] private GameObject _selectedSlotMark;
        [SerializeField] private QuestInventorySlotController _slotHandler;
        private QuestInventorySlot _slotSelected;
        [SerializeField] private Button _complete;
        [SerializeField] private Button _menu;
        [SerializeField] private Button _openInfo;


        // CheckList
        [SerializeField] private Button _checkList;
        [SerializeField] private GameObject _hashtagsRequireListPanel;
        [SerializeField] private GameObject _statsRequiredListPanel;
        [SerializeField] private HashtagSlot _hashtagSlotPrefab;
        [SerializeField] private Transform _hashtagsRequiredContent;
        private SimpleGameObjectPool<HashtagSlot> _hashtagSlotsPool;
        private Dictionary<int, int> _hashtagPairs = new Dictionary<int, int>();
        [SerializeField] private Slider _stat1RequiredBar;
        [SerializeField] private Slider _stat2RequiredBar;
        [SerializeField] private Slider _stat3RequiredBar;
        [SerializeField] private Slider _stat4RequiredBar;
        [SerializeField] private RectTransform _stat1RequiredMark;
        [SerializeField] private RectTransform _stat2RequiredMark;
        [SerializeField] private RectTransform _stat3RequiredMark;
        [SerializeField] private RectTransform _stat4RequiredMark;
        [SerializeField] private Image _colorPreference;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void BeginSlotHandle(QuestInventorySlot slot)
        {
            _slotHandler.Handle(slot);
        }

        public void SelectSlot(QuestInventorySlot slot)
        {
            if (_slotSelected != null)
                _slotSelected.Deselect();
            _slotSelected = slot;
            _selectedSlotMark.transform.SetParent(slot.transform);
            _selectedSlotMark.transform.localPosition = Vector2.zero;
            _selectedSlotMark.SetActive(true);
            RefreshPreviewModel(slot.id);
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
            base.Show();
            _budget.text = QuestManager.instance.budget.GetSimplifiedString();
            _calc.text = QuestManager.instance.calc.GetSimplifiedString();
            SetInventorySlot();
            SetCheckList();
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        override protected void Init()
        {
            slotPool = new SimpleGameObjectPool<QuestInventorySlot>(_slotPrefab, _itemsContent);
            _hashtagSlotsPool = new SimpleGameObjectPool<HashtagSlot>(_hashtagSlotPrefab, _hashtagsRequiredContent);
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestManager.instance != null);
                QuestManager questManager = QuestManager.instance;
                questManager.OnCalcChanged += (gold) =>
                {
                    _calc.text = gold.GetSimplifiedString();
                    if (questManager.calc > questManager.budget)
                        _calc.color = Color.red;
                    else
                        _calc.color = Color.white;
                };

                _complete.onClick.AddListener(() =>
                {
                    if (questManager.budget >= questManager.calc)
                        questManager.Complete();
                    else
                        WarningWindowPopUpUI.instance.Show("예산 부족!", "배치한 가구의 가격 총합이 의뢰인의 예산보다 많아서 의뢰를 완료할 수 없어요.");
                });

                _checkList.onClick.AddListener(() =>
                {
                    _hashtagsRequireListPanel.SetActive(!_hashtagsRequireListPanel.activeSelf);
                    _statsRequiredListPanel.SetActive(!_statsRequiredListPanel.activeSelf);
                });

                // 아이템 추가시
                questManager.items.ItemAdded += (item) =>
                {
                    // 해시태그 갱신
                    foreach (Hashtag hashtag in ItemAssets.instance[item.id.value].hashtags)
                    {
                        if (_hashtagPairs.ContainsKey(hashtag.index))
                        {
                            _hashtagPairs[hashtag.index]++;
                        }
                        else
                        {
                            _hashtagPairs.Add(hashtag.index, 1);
                            foreach (HashtagSlot slot in _hashtagSlotsPool.GetSpawnedObjects())
                            {
                                if (hashtag.index == slot.hashtagIndex)
                                {
                                    slot.Show(hashtag.index, true);
                                    break;
                                }
                            }
                        }
                    }

                    // 스탯 갱신
                    _stat1RequiredBar.value = Player.instance.currentRoom.stats[0].value;
                    _stat2RequiredBar.value = Player.instance.currentRoom.stats[1].value;
                    _stat3RequiredBar.value = Player.instance.currentRoom.stats[2].value;
                    _stat4RequiredBar.value = Player.instance.currentRoom.stats[3].value;
                };

                // 아이템 제거시
                questManager.items.ItemRemoved += (item) =>
                {
                    // 해시태그 갱신
                    foreach (Hashtag hashtag in ItemAssets.instance[item.id.value].hashtags)
                    {
                        foreach (HashtagSlot slot in _hashtagSlotsPool.GetSpawnedObjects())
                        {
                            if (hashtag.index == slot.hashtagIndex)
                            {
                                _hashtagPairs[hashtag.index]--;

                                if (_hashtagPairs[hashtag.index] <= 0)
                                {
                                    _hashtagPairs.Remove(hashtag.index);
                                    slot.Show(hashtag.index, false);
                                }
                                break;
                            }
                        }
                    }

                    // 스탯 갱신
                    _stat1RequiredBar.value = Player.instance.currentRoom.stats[0].value;
                    _stat2RequiredBar.value = Player.instance.currentRoom.stats[1].value;
                    _stat3RequiredBar.value = Player.instance.currentRoom.stats[2].value;
                    _stat4RequiredBar.value = Player.instance.currentRoom.stats[3].value;
                };

                await UniTask.WaitUntil(() => ItemInfoUI.instance != null);
                _openInfo.onClick.AddListener(() =>
                {
                    if (_slotSelected != null)
                        ItemInfoUI.instance.Show(ItemAssets.instance[_slotSelected.id], Vector2.zero);
                });
            });

            base.HideUnmanaged();
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void SetInventorySlot()
        {

            List<int> shoppingList = QuestManager.instance.Current.itemShoppingList;
            using (IEnumerator<int> e1 = shoppingList.GetEnumerator())

            using (IEnumerator<QuestInventorySlot> e2 = slotPool.Refresh(shoppingList.Count).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.id = e1.Current;
                }
            }
            Vector2 slotSize = _slotPrefab.GetComponent<RectTransform>().rect.size;
            _itemsContent.sizeDelta = new Vector2(_itemsContent.sizeDelta.x, (shoppingList.Count + 1) * slotSize.y / 3);
        }

        private void SetCheckList()
        {
            QuestManager questManager = QuestManager.instance;
            RefreshHashtagSlots(questManager.Current.hashtagsRequired);
            _stat1RequiredBar.minValue = Player.instance.currentRoom.stats[0].min;
            _stat1RequiredBar.maxValue = Player.instance.currentRoom.stats[0].max;
            _stat2RequiredBar.minValue = Player.instance.currentRoom.stats[1].min;
            _stat2RequiredBar.maxValue = Player.instance.currentRoom.stats[1].max;
            _stat3RequiredBar.minValue = Player.instance.currentRoom.stats[2].min;
            _stat3RequiredBar.maxValue = Player.instance.currentRoom.stats[2].max;
            _stat4RequiredBar.minValue = Player.instance.currentRoom.stats[3].min;
            _stat4RequiredBar.maxValue = Player.instance.currentRoom.stats[3].max;

            float width = _stat1RequiredBar.GetComponent<RectTransform>().sizeDelta.x;
            _stat1RequiredMark.position = _stat1RequiredBar.transform.position
                                        + (Vector3.left * (width) * questManager.Current.statsRequired[0]
                                            / (_stat1RequiredBar.maxValue - _stat1RequiredBar.minValue));
            _stat2RequiredMark.position = _stat2RequiredBar.transform.position
                                        + (Vector3.left * (width) * questManager.Current.statsRequired[1]
                                            / (_stat2RequiredBar.maxValue - _stat2RequiredBar.minValue));
            _stat3RequiredMark.position = _stat3RequiredBar.transform.position
                                        + (Vector3.left * (width) * questManager.Current.statsRequired[2]
                                            / (_stat3RequiredBar.maxValue - _stat3RequiredBar.minValue));
            _stat4RequiredMark.position = _stat4RequiredBar.transform.position
                                        + (Vector3.left * (width) * questManager.Current.statsRequired[3]
                                            / (_stat4RequiredBar.maxValue - _stat4RequiredBar.minValue));

            _colorPreference.color = questManager.Current.colorRequired;
        }

        private void RefreshHashtagSlots(ICollection<int> hashtagIndexes)
        {
            using (IEnumerator<int> e1 = hashtagIndexes.GetEnumerator())
            using (IEnumerator<HashtagSlot> e2 = _hashtagSlotsPool.Refresh(hashtagIndexes.Count).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.Show(e1.Current, false);
                }
            }
        }
    }
}