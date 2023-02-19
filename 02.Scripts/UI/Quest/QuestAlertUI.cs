using UnityEngine;
using TMPro;
using HTH.DataDependencySources;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using HTH.WorldElements;
using HTH.GameSystems;
using HTH.DataModels;
using Unity.VisualScripting;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 의뢰 발생 알림
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class QuestAlertUI : UIMonoBehaviour<QuestAlertUI>
    {
        [SerializeField] private Transform _alertPanel;
        [SerializeField] private TMP_Text _npcName;
        [SerializeField] private Image _npcIcon;
        [SerializeField] private float _animationSpeed = 10.0f;
        private Vector3 _posOrigin;
        private Vector3 _posPop => _posOrigin + Vector3.down * 400.0f;
        private QuestsPendingPresenter _questPendingPresenter;
        private Queue<int> _queue = new Queue<int>();
        private bool _corouting;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        new public void ShowUnmanaged(int questID)
        {
            base.ShowUnmanaged();
            _queue.Enqueue(questID);
            Debug.Log($"[QuestAlertUI] : {questID} 가 알림 대기열에 등록됨");
            if (_corouting == false)
                QuestManager.instance.StartCoroutine(E_PopAnimation());
        }

        public override void HideUnmanaged()
        {
            base.HideUnmanaged();
            _corouting = false;
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            _posOrigin = _alertPanel.transform.localPosition;
            _questPendingPresenter = new QuestsPendingPresenter();

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _questPendingPresenter.source != null);
                _questPendingPresenter.source.ItemAdded += (questID) =>
                {
                    QuestManager.instance.StartCoroutine(E_WaitUntilPlayerIsReadyAndShowUnManaged(questID));
                };
            });            

            base.Init();
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private IEnumerator E_PopAnimation()
        {
            _corouting = true;

            yield return new WaitUntil(() => QuestManager.instance.step == QuestManager.Step.Idle);

            while (_queue.Count > 0)
            {
                int questID = _queue.Dequeue();
                Debug.Log($"[QuestAlertUI] : {questID} 발생 알림중...");
                _npcIcon.sprite = NPCIconAssets.instance[QuestAssets.instance[questID].npcId];
                _npcName.text = NPCAssets.instance[QuestAssets.instance[questID].npcId.value].name;
                _alertPanel.localPosition = _posOrigin;

                while (_alertPanel.localPosition.y > _posPop.y)
                {
                    _alertPanel.localPosition += Vector3.down * _animationSpeed;
                    yield return null;
                }

                yield return new WaitForSeconds(2.0f);

                while (_alertPanel.localPosition.y < _posOrigin.y)
                {
                    _alertPanel.localPosition += Vector3.up * _animationSpeed;
                    yield return null;
                }
            }
            //base.HideUnmanaged();
            _corouting = false;
        }

        private IEnumerator E_WaitUntilPlayerIsReadyAndShowUnManaged(int questID)
        {
            yield return new WaitUntil(() => Player.instance.isInMyRoom || Player.instance.isOutside);
            ShowUnmanaged(questID);
        }
    }
}