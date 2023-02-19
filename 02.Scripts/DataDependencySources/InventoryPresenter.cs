using Cysharp.Threading.Tasks;
using HTH.DataModels;
using HTH.DataStructures;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : InventoryData Presenter.
    /// </summary>
    public class InventoryPresenter : CollectionDependedObjectModelBase<InventoryData, ItemPair>
    {
        public InventoryPresenter()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => InventoryData.instance != null);
                InitializeSource(InventoryData.instance);
            });
        }
    }
}