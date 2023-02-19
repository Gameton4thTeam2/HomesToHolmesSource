using Cysharp.Threading.Tasks;
using HTH.DataModels;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_13
    /// 설명    : PlayerRoomsData Presenter.
    /// </summary>
    public class PlayerRoomsPresenter : CollectionDependedObjectModelBase<PlayerRoomsData, RoomData>
    {
        public PlayerRoomsPresenter()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => PlayerRoomsData.instance != null);
                InitializeSource(PlayerRoomsData.instance);
            });
        }
    }
}