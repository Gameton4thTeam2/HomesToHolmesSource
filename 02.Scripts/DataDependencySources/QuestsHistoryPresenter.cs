using Cysharp.Threading.Tasks;
using HTH.DataModels;
using HTH.DataStructures;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_20
    /// 설명    : 퀘스트 이력 presenter
    /// </summary>
    public class QuestsHistoryPresenter : CollectionDependedObjectModelBase<QuestsHistoryData, QuestHistoryPair>
    {
        public QuestsHistoryPresenter()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestsHistoryData.instance != null);
                InitializeSource(QuestsHistoryData.instance);
            });
        }
    }
}
