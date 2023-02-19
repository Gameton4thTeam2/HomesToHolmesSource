using Cysharp.Threading.Tasks;
using HTH.DataDependencySources;
using HTH.DataModels;
using HTH.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_17
    /// 설명    : 의뢰함 UI
    /// </summary>
    public class QuestBoxUI : UIMonoBehaviour<QuestBoxUI>
    {
        private QuestsPresenter _presenter;
        [SerializeField] private RectTransform _content;
        [SerializeField] private QuestBoxSlot _slotPrefab;
        private SimpleGameObjectPool<QuestBoxSlot> _pendingSlotPool;
        private SimpleGameObjectPool<QuestBoxSlot> _acceptedSlotPool;
        private float _slotHeight;

        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public override void Show()
        {
            Refresh();
            base.Show();
        }

        public void Refresh()
        {
            RefreshPendingSlots(_presenter.pendingSource);
            RefreshAcceptedSlots(_presenter.acceptedSource);
            _content.sizeDelta = new Vector2(_content.sizeDelta.x,
                                             _slotHeight * (_presenter.pendingSource.Count + _presenter.acceptedSource.Count + 1));
        }


        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        override protected void Init()
        {
            _slotHeight = _slotPrefab.GetComponent<RectTransform>().sizeDelta.y;
            _pendingSlotPool = new SimpleGameObjectPool<QuestBoxSlot>(_slotPrefab, _content);
            _acceptedSlotPool = new SimpleGameObjectPool<QuestBoxSlot>(_slotPrefab, _content);

            _presenter = new QuestsPresenter();

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _presenter.pendingSource != null);
                await UniTask.WaitUntil(() => _presenter.acceptedSource != null);

                _presenter.pendingSource.CollectionChanged += () => RefreshPendingSlots(_presenter.pendingSource); ;
                _presenter.acceptedSource.CollectionChanged += () => RefreshAcceptedSlots(_presenter.acceptedSource);
            }); 
            
            base.HideUnmanaged();
        }


        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        /// <summary>
        /// 대기중 퀘스트 슬롯 갱신. 갱신시 Sibling index 하단으로
        /// </summary>
        private void RefreshPendingSlots(ICollection<int> items)
        {
            using (IEnumerator<int> e1 = items.GetEnumerator())
            using (IEnumerator<QuestBoxSlot> e2 = _pendingSlotPool.Refresh(items.Count).GetEnumerator())
            {
                Debug.Log($"[QuestBoxUI] : Start Refreshpending slots ... total :  {items.Count}");

                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.SetUp(QuestAssets.instance[e1.Current], false);
                    e2.Current.transform.SetAsLastSibling();
                    Debug.Log($"[QuestBoxUI] : Refreshing pending slots ... quest ID :  {e1.Current}");
                }
            }
        }

        /// <summary>
        /// 수락한 퀘스트 슬롯 갱신. 갱신시 Sibling index 상단으로
        /// </summary>
        private void RefreshAcceptedSlots(ICollection<int> items)
        {
            using (IEnumerator<int> e1 = items.GetEnumerator())
            using (IEnumerator<QuestBoxSlot> e2 = _acceptedSlotPool.Refresh(items.Count).GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    e2.Current.SetUp(QuestAssets.instance[e1.Current], true);
                    e2.Current.transform.SetAsFirstSibling();
                    Debug.Log($"[QuestBoxUI] : Refreshing accepted slots ... quest ID :  {e1.Current}");
                }
            }
        }
    }
}