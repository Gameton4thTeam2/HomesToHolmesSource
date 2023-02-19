using Cysharp.Threading.Tasks;
using HTH.DataModels;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 진행중 퀘스트 Presenter
    /// </summary>
    public class QuestsInProgressPresenter : CollectionDependedObjectModelBase<QuestsInProgressData, int>
    {
        public QuestsInProgressPresenter()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestsInProgressData.instance != null);
                InitializeSource(QuestsInProgressData.instance);
            });
        }
    }
}
