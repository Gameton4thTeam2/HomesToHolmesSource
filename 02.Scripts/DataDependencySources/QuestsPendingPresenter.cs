using Cysharp.Threading.Tasks;
using HTH.DataModels;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_20
    /// 설명    : 보류중 퀘스트 Presenter
    /// </summary>
    public class QuestsPendingPresenter : CollectionDependedObjectModelBase<QuestsPendingData, int>
    {
        public QuestsPendingPresenter()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestsPendingData.instance != null);
                InitializeSource(QuestsPendingData.instance);
            });
        }
    }
}
