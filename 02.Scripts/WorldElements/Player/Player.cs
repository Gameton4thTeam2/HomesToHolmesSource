using HTH.InputHandlers;
using System.Collections;
using UnityEngine;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_12
    /// 설명    : 맵 이동 시뮬레이션용 플레이어
    /// </summary>
    public class Player : SingletonMonoBase<Player>
    {
        public bool isInMyRoom => currentRoom != null && PlayerRoomsManager.instance.rooms.Contains(currentRoom);
        public bool isOutside => currentRoom == null;
        public Building currentBuilding;
        public Room currentRoom;
        public GameObject world;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void ExitRoom()
        {
            if (currentBuilding != null &&
                currentRoom != null)
            {
                MoveTo(currentBuilding);
            }
        }

        public void MoveTo(Building building)
        {
            world.SetActive(true);
            currentBuilding = building;
            currentRoom = null;
            MoveTo(building.transform.position);
            CameraController.instance.SetTarget(building.transform);
        }

        public void MoveTo(Room room)
        {
            world.SetActive(false);
            currentRoom = room;
            MoveTo(room.transform.position);
            CameraController.instance.SetTarget(room.transform);
            GridSnappingHelper.instance.SetRoom(room.transform);
        }

        public void MoveTo(Vector3 position)
        {
            transform.position = position;
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            base.Init();
            StartCoroutine(E_Init());
        }

        IEnumerator E_Init()
        {
            yield return new WaitUntil(() => PlayerRoomsManager.instance != null);
            yield return new WaitUntil(() => PlayerRoomsManager.instance.rooms.Count > 0);
            MoveTo(PlayerRoomsManager.instance.rooms.First.Value);
        }
    }
}