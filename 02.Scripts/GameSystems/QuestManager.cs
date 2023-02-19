using Cysharp.Threading.Tasks;
using HTH.DataDependencySources;
using HTH.DataModels;
using HTH.DataStructures;
using HTH.UI;
using HTH.WorldElements;
using HTH.AISystems;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HTH.GameSystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_12
    /// 설명    : 메인퀘스트의 로직을 관리하는 매니저 클래스
    /// </summary>
    public class QuestManager : SingletonMonoBase<QuestManager>
    {
        private QuestsPendingPresenter _questsPendingPresenter;
        private QuestsAcceptedPresenter _questsAcceptedPresenter;
        private QuestsInProgressPresenter _questsInProgressPresenter;
        private QuestsHistoryPresenter _questHistoryPresenter;
        private InventoryPresenter _inventoryPresenter;
        private BehaviourTree _questEventTree;

        public enum Step
        {
            Idle,
            Start,
            OnProgress,
            Complete,
            WaitForUserAction
        }
        public Step step
        {
            get
            {
                return _step;
            }
            set
            {
                if (_step == value)
                    return;

                switch (value)
                {
                    case Step.Idle:
                        Current = null;
                        break;
                    case Step.Start:
                        break;
                    case Step.OnProgress:
                        QuestRoomUI.instance.Show();
                        break;
                    case Step.Complete:
                        break;
                    case Step.WaitForUserAction:
                        QuestResultUI.instance.Show(_rewardGold,_practicalityScore,_colorScore,_preferenceScore, _rewardItems, CalcRank());
                        break;
                    default:
                        break;
                }
                _step = value;
            }
        }
        private Step _step;

        [HideInInspector] public QuestInfo Current;
        private Vector3 _questRoomWolrdPos = new Vector3(-2000.0f, -2000.0f, -2000.0f);
        public Gold budget => Current.budget;
        public Gold calc
        {
            get
            {
                return _calc;
            }
            set
            {
                _calc = value;
                OnCalcChanged?.Invoke(value);
            }
        }
        [SerializeField] private Gold _calc;
        public event Action<Gold> OnCalcChanged;
        public Collections.ObservableCollection<Item> items = new Collections.ObservableCollection<Item>();
        private Gold _rewardGold;
        private List<UKeyValuePair<int, int>> _rewardItems;
        private Room _room;
        private float _practicalityScore;
        private float _colorScore;
        private float _preferenceScore;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        /// <summary>
        /// 퀘스트 시작시 호출
        /// </summary>
        public void StartQuest(QuestInfo questInfo)
        {
            // 모든 UI 숨김
            UIManager.instance.HideAll();

            // 퀘스트용 방만듬
            _room = Room.Create(questInfo.roomData, _questRoomWolrdPos);

            // 퀘스트용 방으로 플레이어 이동
            Player.instance.MoveTo(_room);

            // 퀘스트 등록
            Current = questInfo;

            // 계산서 초기화
            calc = Gold.zero;

            // 채팅 시작
            if (Current.chatsBeginning != null)
            {                
                ChattingUI.instance.Show(Current.chatsBeginning, null, () => step = Step.OnProgress );
                step = Step.Start;
            }
            else
            {
                step = Step.OnProgress;
            }
        }

        /// <summary>
        /// 퀘스트 다했을때 유저가 호출하는 함수
        /// </summary>
        public void Complete()
        {
            // 랭크별 보상 정산
            Rank historyRank = _questHistoryPresenter.source.Find(history => history.questID == Current.id.value).rank;
            Rank currentRank = CalcRank();

            _rewardGold = Current.rewardGold - Current.GetRewardGoldByRank(historyRank);
            _rewardItems = new List<UKeyValuePair<int, int>>();
            for (Rank r = historyRank + 1; r <= currentRank; r++)
            {
                switch (r)
                {
                    case Rank.F:
                        break;
                    case Rank.E:
                        _rewardItems.AddRange(Current.rewardItems_RankE);
                        break;
                    case Rank.D:
                        _rewardItems.AddRange(Current.rewardItems_RankD);
                        break;
                    case Rank.C:
                        _rewardItems.AddRange(Current.rewardItems_RankC);
                        break;
                    case Rank.B:
                        _rewardItems.AddRange(Current.rewardItems_RankB);
                        break;
                    case Rank.A:
                        _rewardItems.AddRange(Current.rewardItems_RankA);
                        break;
                    case Rank.S:
                        _rewardItems.AddRange(Current.rewardItems_RankS);
                        break;
                    default:
                        break;
                }
            }

            // 퀘스트 데이터 제거
            _questsAcceptedPresenter.removeCommand.Execute(Current.id.value);
            _questHistoryPresenter.addCommand.Execute(new QuestHistoryPair(Current.id.value, currentRank));

            // 채팅시작 
            if (Current.chatsEnding != null)
            {
                ChattingUI.instance.Show(Current.chatsEnding, null, () => step = Step.WaitForUserAction);
                step = Step.Complete;
            }
            else
            {
                step = Step.WaitForUserAction;
            }
        }

        /// <summary>
        /// 퀘스트 결과창을 유저가 확인하고닫으려할때 호출
        /// </summary>
        public void FinishQuest()
        {
            // 랭크별 보상획득            
            AssetData.instance.IncreaseGold(_rewardGold);
            foreach (var item in _rewardItems)
            {
                _inventoryPresenter.addCommand.Execute(new ItemPair(item.key, item.value));
            }

            // 리셋
            step = Step.Idle;

            // 모든 UI 숨기기
            UIManager.instance.HideAll();

            // 플레이어가 방 나가기
            Player.instance.ExitRoom();

            // 의뢰용 임시 방 삭제
            Destroy(_room.gameObject);

            // 월드 UI 활성
            WorldUI.instance.Show();

            // 퀘스트 트리 탐색
            _questEventTree.Run();
        }

        /// <summary>
        /// 새로운 퀘스트 발생
        /// </summary>
        public bool TryGenerateQuest(int questID)
        {
            if (_questsPendingPresenter.source.Contains(questID) ||
                _questsAcceptedPresenter.source.Contains(questID) ||
                _questsInProgressPresenter.source.Contains(questID) ||
                _questHistoryPresenter.source.Find(x => x.questID == questID).rank != Rank.None)
            {
                return false;
            }

            _questsPendingPresenter.addCommand.Execute(questID);
            Debug.Log($"[QuestManager] : 퀘스트 {questID} 가 새로 발생함.");
            return true;
        }

        public void RunEventTree()
        {
            Debug.Log("[QuestManager] : 새롭게 발생 가능한 퀘스트가 있는지 재탐색 시작...");
            _questEventTree.Run();
        }


        //===============================================================================================
        //                                  Private Methods
        //===============================================================================================

        override protected void Init()
        {
            base.Init();
            _questsPendingPresenter = new QuestsPendingPresenter();
            _questsAcceptedPresenter = new QuestsAcceptedPresenter();
            _questsInProgressPresenter = new QuestsInProgressPresenter();
            _questHistoryPresenter = new QuestsHistoryPresenter();
            _inventoryPresenter = new InventoryPresenter();
            _questEventTree = new BehaviourTree();
            
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _questsPendingPresenter.source != null &&
                                              _questsAcceptedPresenter.source != null &&
                                              _questsInProgressPresenter.source != null &&
                                              _questHistoryPresenter.source != null &&
                                              _inventoryPresenter.source != null &&
                                              ChattingUI.instance != null &&
                                              QuestResultUI.instance != null &&
                                              ItemAssets.instance != null &&
                                              Player.instance != null);

                BuildQuestEventTree();
                _questsPendingPresenter.source.CollectionChanged += _questEventTree.Run;
                _questsAcceptedPresenter.source.CollectionChanged += _questEventTree.Run;
                _questsInProgressPresenter.source.CollectionChanged += _questEventTree.Run;
                _questHistoryPresenter.source.CollectionChanged += _questEventTree.Run;

                items.ItemAdded += (item) =>
                {
                    calc += ItemAssets.instance[item.id.value].buyPrice;
                    Player.instance.currentRoom.AddItem(item);
                };
                items.ItemRemoved += (item) =>
                {
                    calc -= ItemAssets.instance[item.id.value].buyPrice;
                    Player.instance.currentRoom.RemoveItem(item);
                };

                ChattingUI.instance.OnChatingFinish += () => step++;
                QuestResultUI.instance.OnConfirmed += () => Player.instance.ExitRoom();
            });            
        }

        /// <summary>
        /// 해시태그, 색감, 선호도 점수로 랭크 산정.
        /// 색감 점수부분은 아직 구현되지 않았음.
        /// </summary>
        /// <returns></returns>
        private Rank CalcRank()
        {
            int hashtagSum = 0;
            IEnumerable<int> hashtagsActivated = QuestRoomUI.instance.hashtagIDs;
            foreach (int hashtagID in Current.hashtagsRequired)
            {
                if (hashtagsActivated.Contains(hashtagID))
                    hashtagSum++;
            }
            _practicalityScore = 40.0f * (float)hashtagSum / Current.hashtagsRequired.Count;

            _colorScore = 20.0f * UnityEngine.Random.Range(0.7f, 1.0f);

            _preferenceScore =
                  10.0f * (1.0f - (Current.statsRequired[0] - Player.instance.currentRoom.stats[0].value) / (Player.instance.currentRoom.stats[0].max - Player.instance.currentRoom.stats[0].min))
                + 10.0f * (1.0f - (Current.statsRequired[1] - Player.instance.currentRoom.stats[1].value) / (Player.instance.currentRoom.stats[1].max - Player.instance.currentRoom.stats[1].min))
                + 10.0f * (1.0f - (Current.statsRequired[2] - Player.instance.currentRoom.stats[2].value) / (Player.instance.currentRoom.stats[2].max - Player.instance.currentRoom.stats[2].min))
                + 10.0f * (1.0f - (Current.statsRequired[3] - Player.instance.currentRoom.stats[3].value) / (Player.instance.currentRoom.stats[3].max - Player.instance.currentRoom.stats[3].min));
            

            float totalScore = _practicalityScore + _colorScore + _preferenceScore;
            Rank rank = Rank.F;

            if (totalScore > 90.0f) rank = Rank.S;
            else if (totalScore > 75.0f) rank = Rank.A;
            else if (totalScore > 60.0f) rank = Rank.B;
            else if (totalScore > 40.0f) rank = Rank.C;
            else if (totalScore > 20.0f) rank = Rank.D;
            else if (totalScore > 10.0f) rank = Rank.E;

            return rank;
        }

        private void BuildQuestEventTree()
        {
            _questEventTree = new BehaviourTree();
            _questEventTree.StartBuild()
                .Parallel(0)
                    .QuestEventGenerator(1001)
                        .QuestHistoryCondition(1001, Rank.C)
                            .QuestEventGenerator(1002)
                                .Success()
                    .QuestEventGenerator(2001)
                        .QuestHistoryCondition(2001, Rank.A)
                            .QuestEventGenerator(2002)
                                .Success()
                    .QuestEventGenerator(8001)
                        .Success()
                    .Sequence()
                        .Parallel(2)
                            .QuestHistoryCondition(1001, Rank.B)
                                .Success()
                            .QuestHistoryCondition(2001, Rank.B)
                                .Success()
                            .QuestHistoryCondition(8001, Rank.B)
                                .Success()
                            .ExitCurrentComposite()
                        .QuestEventGenerator(3001)
                            .QuestHistoryCondition(3001, Rank.B)
                                .QuestEventGenerator(3002)
                                    .Success()
                        .QuestEventGenerator(17001)
                            .QuestHistoryCondition(17001, Rank.A)
                                .QuestEventGenerator(17002)
                                    .Success()
                        .QuestEventGenerator(14001)
                            .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestHistoryCondition(8001, Rank.B)
                            .Success()
                        .QuestHistoryCondition(17001, Rank.B)
                            .Success()
                        .QuestEventGenerator(8002)
                            .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestHistoryCondition(2001, Rank.C)
                            .Success()
                        .QuestHistoryCondition(3002, Rank.B)
                            .Success()
                        .QuestEventGenerator(14002)
                            .QuestHistoryCondition(14002, Rank.A)
                                .QuestEventGenerator(14003)
                                    .Success()
                            .QuestEventGenerator(3003)
                                .Success()
                            .QuestEventGenerator(2003)
                                .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestHistoryCondition(2003, Rank.A)
                            .Success()
                        .QuestHistoryCondition(3003, Rank.A)
                            .Success()
                        .QuestHistoryCondition(14002, Rank.B)
                            .Success()
                        .QuestEventGenerator(17003)
                            .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestEventGenerator(6001)
                            .Success()
                        .QuestEventGenerator(7001)
                            .QuestHistoryCondition(7001, Rank.A)
                                .QuestEventGenerator(7002)
                                    .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestHistoryCondition(14001, Rank.B)
                            .Success()
                        .QuestEventGenerator(10001)
                            .QuestHistoryCondition(10001, Rank.B)
                                .QuestEventGenerator(10002)
                                    .QuestHistoryCondition(10002, Rank.A)
                                        .QuestEventGenerator(10003)
                                            .Success()
                        .QuestEventGenerator(12001)
                            .QuestHistoryCondition(12001, Rank.A)
                                .QuestEventGenerator(12002)
                                    .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestHistoryCondition(14002, Rank.A)
                            .Success()
                        .QuestEventGenerator(6002)
                            .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestHistoryCondition(17003, Rank.B)
                            .Success()
                        .QuestHistoryCondition(7001, Rank.A)
                            .Success()
                        .QuestEventGenerator(7003)
                            .Success()
                        .ExitCurrentComposite()
                    .Sequence()
                        .QuestHistoryCondition(12002, Rank.B)
                            .Success()
                        .QuestHistoryCondition(17002, Rank.B)
                            .Success()
                        .QuestEventGenerator(12003)
                            .Success()
                        .ExitCurrentComposite();
        }
    }
}