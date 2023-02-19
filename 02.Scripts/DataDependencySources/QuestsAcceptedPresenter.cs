using Cysharp.Threading.Tasks;
using HTH.DataModels;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_20
    /// 설명    : 수락한 퀘스트 presenter
    /// </summary>
    public class QuestsAcceptedPresenter : CollectionDependedObjectModelBase<QuestsAcceptedData, int>
    {
        public QuestsAcceptedPresenter()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestsAcceptedData.instance != null);
                InitializeSource(QuestsAcceptedData.instance);
            });            
        }
    }
}
