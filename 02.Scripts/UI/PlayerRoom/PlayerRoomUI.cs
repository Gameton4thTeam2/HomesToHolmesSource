using Cysharp.Threading.Tasks;
using HTH.WorldElements;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자      : 조영민
    /// 작성일      : 2023_01_16
    /// 설명        : 플레이어가 방으로 왔을때 보여질 UI
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_09
    /// 설명        : 메뉴 버튼 추가[2023_01_19]
    ///               출석 이벤트 버튼 추가
    /// </summary>
    public class PlayerRoomUI : UIMonoBehaviour<PlayerRoomUI>
    {
        [SerializeField] private Button _openInventoryUI;
        [SerializeField] private Button _openItemShopUI;
        [SerializeField] private Button _gotoWorld;
        [SerializeField] private Button _prev;
        [SerializeField] private Button _next;
        [SerializeField] private Button _controlMenu;
        [SerializeField] private Button _attendance;


        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        override protected void Init()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => MenuUI.instance != null);
                _controlMenu.onClick.AddListener(() => MenuUI.instance.Toggle());

                await UniTask.WaitUntil(() => InventoryUI.instance != null);
                _openInventoryUI.onClick.AddListener(InventoryUI.instance.ShowUnmanaged);

                await UniTask.WaitUntil(() => ItemShopUI.instance != null);
                _openItemShopUI.onClick.AddListener(ItemShopUI.instance.Show);

                await UniTask.WaitUntil(() => BuildingManager.instance.playerBuilding != null);
                await UniTask.WaitUntil(() => Player.instance != null);
                _gotoWorld.onClick.AddListener(() =>
                {
                    Hide();
                    Player.instance.MoveTo(BuildingManager.instance.playerBuilding);
                    WorldUI.instance.Show();
                });
                _prev.onClick.AddListener(() =>
                {
                    if (PlayerRoomsManager.instance.TryGetPrevRoom(Player.instance.currentRoom, out Room prev))
                    {
                        Player.instance.MoveTo(prev);
                    }
                });
                _next.onClick.AddListener(() =>
                {
                    if (PlayerRoomsManager.instance.TryGetNextRoom(Player.instance.currentRoom, out Room next))
                    {
                        Player.instance.MoveTo(next);
                    }
                });
                await UniTask.WaitUntil(() => AttendanceUI.instance != null);
                _attendance.onClick.AddListener(AttendanceUI.instance.Show);
            });

            HideUnmanaged();
        }
    }
}