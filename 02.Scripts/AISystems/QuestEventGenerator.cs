using Cysharp.Threading.Tasks;
using HTH.DataDependencySources;
using HTH.GameSystems;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 퀘스트 이벤트를 발생시킴. 이미 발생되었다면 그뒤부터는 발생시도하지 않고 자식 탐색. 
    /// 발생한 이력이 있는 상태에서만 자식행동 탐색
    public class QuestEventGenerator : Behaviour, IChild
    {
        public Behaviour child { get; set; }
        private int _questID;
        private bool _isGenerated;
        private QuestsPresenter _questPresenter;
        private QuestsHistoryPresenter _historyPresenter;

        public QuestEventGenerator(int questID)
        {
            _questID = questID;
            _questPresenter = new QuestsPresenter();
            _historyPresenter = new QuestsHistoryPresenter();
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _questPresenter.pendingSource != null &&
                                              _questPresenter.acceptedSource != null &&
                                              _questPresenter.inProgressSource != null);
                await UniTask.WaitUntil(() => _historyPresenter.source != null);

                if (_questPresenter.pendingSource.FindIndex(x => x == questID) >= 0 ||
                    _questPresenter.acceptedSource.FindIndex(x => x == questID) >= 0 ||
                    _questPresenter.inProgressSource.FindIndex(x => x == questID) >= 0 ||
                    _historyPresenter.source.FindIndex(x => x.questID == questID) >= 0)
                {
                    _isGenerated = true;
                }
            });
        }

        public override async UniTask<Result> Invoke()
        {
            Result result = Result.Failure;
            if (_isGenerated == false &&
                QuestManager.instance.TryGenerateQuest(_questID))
            {
                UnityEngine.Debug.Log($"[BehaviourTree][QuestEventGenerator] : Quest event gernerator invoked {_questID}");
                _isGenerated = true;
            }
            else
            {
                UnityEngine.Debug.Log($"[BehaviourTree][QuestEventGenerator] : Quest {_questID} has already generated");
            }

            if (_isGenerated)
            {
                result = await child.Invoke();
            }

            return result;
        }
    }
}
