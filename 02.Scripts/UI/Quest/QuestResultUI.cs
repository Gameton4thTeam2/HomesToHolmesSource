using UnityEngine;
using TMPro;
using HTH.DataModels;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using HTH.GameSystems;
using HTH.DataStructures;
using HTH.IDs;
using System.Linq;
using HTH.Tools;
using Cysharp.Threading.Tasks;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 퀘스트 종료 UI
    /// </summary>
    public class QuestResultUI : UIMonoBehaviour<QuestResultUI>
    {
        [SerializeField] private Slider _practicalityRate;
        [SerializeField] private Slider _colorRate;
        [SerializeField] private Slider _preferenceRate;
        [SerializeField] private TMP_Text _practicality;
        [SerializeField] private TMP_Text _color;
        [SerializeField] private TMP_Text _preference;
        [SerializeField] private Transform _rewardItemContent;
        [SerializeField] private RewardPreviewSlot _rewardItemSlotPrefab;
        private SimpleGameObjectPool<RewardPreviewSlot> _rewardSlotPool;
        [SerializeField] private Image _rank;
        [SerializeField] private List<UKeyValuePair<Rank, Sprite>> _rankIcons;
        [SerializeField] private Button _confirmButton;
        public event UnityAction OnConfirmed
        {
            add
            {
                _confirmButton.onClick.AddListener(value);
            }
            remove
            {
                _confirmButton.onClick.RemoveListener(value);
            }
        }


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void Show(Gold rewardGold, float practicalityScore, float colorScore, float preferenceScore, IEnumerable<UKeyValuePair<int, int>> rewardItems, Rank result)
        {
            SetUpRewardGoldSlot(rewardGold);
            SetUpRewardItemsSlots(rewardItems);
            _rank.sprite = _rankIcons.Find(x => x.key == result).value;
            _practicalityRate.value = practicalityScore / 40.0f;
            _colorRate.value = colorScore / 20.0f;
            _preferenceRate.value = preferenceScore / 40.0f;
            _practicality.text = System.Math.Round((_practicalityRate.value * 100), 2).ToString();
            _color.text =  System.Math.Round((_colorRate.value * 100), 2).ToString();
            _preference.text = System.Math.Round((_preferenceRate.value * 100), 2).ToString();

            base.Show();
        }

        /// <summary>
        /// todo -> don't destroy slots but do pooling slots
        /// </summary>
        public override void Hide()
        {
            for (int i = _rewardItemContent.childCount - 1; i >= 0 ; i--)
            {
                Destroy(_rewardItemContent.GetChild(i).gameObject);
            }
            base.Hide();
        }


        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        override protected void Init()
        {
            _rewardSlotPool = new SimpleGameObjectPool<RewardPreviewSlot>(_rewardItemSlotPrefab);
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestManager.instance != null);
                OnConfirmed += () =>
                {
                    Hide();
                    QuestManager.instance.FinishQuest();
                    
                };
            });
                        
            base.HideUnmanaged();
        }


        //===============================================================================================
        //                                  Private Methods
        //===============================================================================================

        private void SetUpRewardItemsSlots(IEnumerable<UKeyValuePair<int, int>> itemPairs)
        {
            using (IEnumerator<UKeyValuePair<int, int>> e1 = itemPairs.GetEnumerator())
            using (IEnumerator<RewardPreviewSlot> e2 = _rewardSlotPool.Spawn(itemPairs.Count()).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.SetUp(new ItemPair(e1.Current.key, e1.Current.value));
                }
            }
        }

        private void SetUpRewardGoldSlot(Gold gold)
        {
            _rewardSlotPool.Spawn(1).First().SetUp(gold);
        }
    }
}