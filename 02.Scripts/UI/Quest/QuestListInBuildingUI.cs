using Cysharp.Threading.Tasks;
using HTH.DataDependencySources;
using HTH.DataModels;
using HTH.IDs;
using HTH.Tools;
using HTH.WorldElements;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_15
    /// 설명    : 수락한 의뢰들중 특정 건물위치만 선택한 목록을 띄우는 UI
    /// </summary>
    public class QuestListInBuildingUI : UIMonoBehaviour<QuestListInBuildingUI>
    {
        [SerializeField] private Transform _content;
        [SerializeField] private QuestSlot _slotPrefab;
        private SimpleGameObjectPool<QuestSlot> _slotsPool;
        private QuestsAcceptedPresenter _acceptedPresenter;

        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void Show(Building building)
        {
            RefreshSlots(_acceptedPresenter.source
                        .Where(id => NPCAssets.instance[QuestAssets.instance[id].npcId.value].buildilngID.value == building.id.value)
                        .Select(id => QuestAssets.instance[id]).ToList());
            base.Show();
        }


        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        override protected void Init()
        {
            _slotsPool = new SimpleGameObjectPool<QuestSlot>(_slotPrefab, _content);
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _acceptedPresenter.source != null);
            });
            _acceptedPresenter = new QuestsAcceptedPresenter();
            base.HideUnmanaged();
        }


        //===============================================================================================
        //                                  Private Methods
        //===============================================================================================

        /// <summary>
        /// 슬롯 전부 갱신
        /// </summary>
        private void RefreshSlots(ICollection<QuestInfo> items)
        {
            using (IEnumerator<QuestInfo> e1 = items.GetEnumerator())
            using (IEnumerator<QuestSlot> e2 = _slotsPool.Refresh(items.Count).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.SetUp(e1.Current);
                }
            }
        }
    }
}