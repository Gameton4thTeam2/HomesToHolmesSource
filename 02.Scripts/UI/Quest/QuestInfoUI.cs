using UnityEngine;
using TMPro;
using HTH.DataModels;
using System.Collections.Generic;
using HTH.IDs;
using HTH.Tools;
using HTH.DataStructures;
using System.Linq;
using UnityEngine.UI;
using HTH.GameSystems;
using Cysharp.Threading.Tasks;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 퀘스트 정보 UI
    /// </summary>
    public class QuestInfoUI : UIMonoBehaviour<QuestInfoUI>
    {
        [SerializeField] private Image _npcIcon;
        [SerializeField] private TMP_Text _npcName;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _colorPreview;
        [SerializeField] private HashtagSlot _hashtagSlotPrefab;
        [SerializeField] private Transform _hashtagsRequiredContent;
        [SerializeField] private Transform _furnituresRequiredContent;
        [SerializeField] private FurniturePreviewSlot _furniturePreviewSlotPrefab;
        [SerializeField] private Transform _rewardContent;
        [SerializeField] private RewardPreviewSlot _rewardPreviewSlotPrefab;
        [SerializeField] private Button _startButton;

        private SimpleGameObjectPool<HashtagSlot> _hashtagSlotsPool;
        private SimpleGameObjectPool<FurniturePreviewSlot> _furniturePreviewSlotsPool;            
        private SimpleGameObjectPool<RewardPreviewSlot> _rewardPreviewSlotsPool;
        private QuestInfo _current;

        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Show(QuestInfo questInfo)
        {
            _npcIcon.sprite = NPCIconAssets.instance[questInfo.npcId];
            _npcName.text = NPCAssets.instance[questInfo.npcId.value].name;
            _title.text = questInfo.title;
            _description.text = questInfo.description;

            // 색감 미리보기 세팅
            //------------------------------------------------------
            _colorPreview.color = questInfo.colorRequired;

            // 필수 요구 해시태그 미리보기 슬롯들 세팅
            //------------------------------------------------------
            SetUpRequiredHashtagSlots(questInfo.hashtagsRequired);
            //SetUpRequiredFurniturePreviewSlots(questInfo.hashtagsRequired);

            // 보상 미리보기 슬롯들 세팅
            //------------------------------------------------------
            _rewardPreviewSlotsPool.ReturnAll();

            // 골드 보상 == 최고랭크보상 - 이전에획득한랭크보상
            SetUpRewardGoldPreviewSlot(questInfo.rewardGold - questInfo.GetRewardGoldByRank(((QuestsHistoryData)QuestsHistoryData.instance).GetRank(questInfo.id.value)));

            // 랭크별 아이템 보상
            SetUpRewardItemsPreviewSlots(questInfo.rewardItems_RankS);
            SetUpRewardItemsPreviewSlots(questInfo.rewardItems_RankA);
            SetUpRewardItemsPreviewSlots(questInfo.rewardItems_RankB);
            SetUpRewardItemsPreviewSlots(questInfo.rewardItems_RankC);
            SetUpRewardItemsPreviewSlots(questInfo.rewardItems_RankD);
            SetUpRewardItemsPreviewSlots(questInfo.rewardItems_RankE);

            _current = questInfo;
            base.Show();
        }


        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        override protected void Init()
        {
            _hashtagSlotsPool
                = new SimpleGameObjectPool<HashtagSlot>(_hashtagSlotPrefab, _hashtagsRequiredContent);
            _furniturePreviewSlotsPool
                = new SimpleGameObjectPool<FurniturePreviewSlot>(_furniturePreviewSlotPrefab, _furnituresRequiredContent);
            _rewardPreviewSlotsPool
                = new SimpleGameObjectPool<RewardPreviewSlot>(_rewardPreviewSlotPrefab, _rewardContent);

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestListInBuildingUI.instance != null &&
                                              QuestManager.instance != null);
                _startButton.onClick.AddListener(() =>
                {
                    if (_current != null)
                    {
                        Hide();
                        // Q. Best way to hide previous ui not associated?
                        if (QuestListInBuildingUI.instance.gameObject.activeSelf)
                            QuestListInBuildingUI.instance.Hide();

                        QuestManager.instance.StartQuest(_current);
                    }
                    else
                    {
                        WarningWindowPopUpUI.instance.Show("Error : Selected Quest is invalid", Hide);
                    }
                });
            });
            
            base.HideUnmanaged();
        }


        //===============================================================================================
        //                                  Private Methods
        //===============================================================================================

        private void SetUpRequiredHashtagSlots(IEnumerable<int> hashtagIDs)
        {
            using (IEnumerator<int> e1 = hashtagIDs.GetEnumerator())
            using (IEnumerator<HashtagSlot> e2 = _hashtagSlotsPool.Refresh(hashtagIDs.Count()).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.Show(e1.Current, false);
                }
            }
        }

        private void SetUpRequiredFurniturePreviewSlots(IEnumerable<int> hashtags)
        {
            using (IEnumerator<int> e1 = hashtags.GetEnumerator())
            using (IEnumerator<FurniturePreviewSlot> e2 = _furniturePreviewSlotsPool.Refresh(hashtags.Count()).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.SetUp(ItemAssets.instance[e1.Current].icon);
                }
            }
        }

        private void SetUpRewardItemsPreviewSlots(IEnumerable<UKeyValuePair<int, int>> itemPairs)
        {
            using (IEnumerator<UKeyValuePair<int, int>> e1 = itemPairs.GetEnumerator())
            using (IEnumerator<RewardPreviewSlot> e2 = _rewardPreviewSlotsPool.Spawn(itemPairs.Count()).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.SetUp(new ItemPair(e1.Current.key, e1.Current.value));
                }
            }
        }

        private void SetUpRewardGoldPreviewSlot(Gold gold)
        {
            _rewardPreviewSlotsPool.Spawn(1).First().SetUp(gold);
        }
    }
}