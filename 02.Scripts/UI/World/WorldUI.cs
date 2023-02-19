using Cysharp.Threading.Tasks;
using HTH.WorldElements;
using HTH.GameSystems;
using UnityEngine;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_16
    /// 설명    : 플레이어가 월드로 갔을때 보여질 UI
    /// </summary>
    public class WorldUI : UIMonoBehaviour<WorldUI>
    {
        [SerializeField] private Button _openQuestBox;
        [SerializeField] private Button _gotoPlayerRoom;


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        override protected void Init()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestManager.instance != null);
                await UniTask.WaitUntil(() => BuildingManager.instance != null);
                await UniTask.WaitUntil(() => PlayerRoomUI.instance != null);
                await UniTask.WaitUntil(() => QuestBoxUI.instance != null);

                _openQuestBox.onClick.AddListener(() =>
                {
                    QuestBoxUI.instance.Show();
                });

                _gotoPlayerRoom.onClick.AddListener(() =>
                {
                    Hide();
                    Player.instance.MoveTo(BuildingManager.instance.playerBuilding);
                    Player.instance.MoveTo(PlayerRoomsManager.instance.rooms.First.Value);
                    PlayerRoomUI.instance.Show();
                });
            });
            
            HideUnmanaged();       
        }
    }
}