using Cysharp.Threading.Tasks;
using HTH.DataDependencySources;
using HTH.DataStructures;
using HTH.IDs;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 퀘스트 이력 조건 체크
    public class QuestHistroyCondition : Behaviour, IChild
    {
        public Behaviour child { get; set; }
        private int _questIDRequired;
        private Rank _rankRequired;
        private QuestsHistoryPresenter _questHistoryPresenter;
        private bool _meetCondition;

        public QuestHistroyCondition(int questIDRequired, Rank rankRequired)
        {
            _questIDRequired = questIDRequired;
            _rankRequired = rankRequired;
            _questHistoryPresenter = new QuestsHistoryPresenter();
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _questHistoryPresenter.source != null);
                if (_questHistoryPresenter.source.Find(x => x.questID == questIDRequired).rank >= rankRequired)
                {
                    _meetCondition = true;
                };
            });
        }

        public override async UniTask<Result> Invoke()
        {
            UnityEngine.Debug.Log($"[BehaviourTree][QuestHistoryCondition] : {_questIDRequired}, {_rankRequired}");

            Result result = Result.Failure;
            QuestHistoryPair pair = _questHistoryPresenter.source.Find(x => x.questID == _questIDRequired);

            if (_meetCondition)
            {
                result = Result.Success;
            }
            else if (pair.questID == _questIDRequired &&
                     pair.rank > Rank.None &&
                     pair.rank >= _rankRequired)
            {
                _meetCondition = true;
                result = await child.Invoke();
            }

            return result;
        }
    }
}
