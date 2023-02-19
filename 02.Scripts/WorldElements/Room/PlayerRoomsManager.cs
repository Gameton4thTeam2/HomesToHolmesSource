using HTH.DataDependencySources;
using UnityEngine;
using System.Collections.Generic;
using System;
using HTH.DataModels;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_12
    /// 설명    : 플레이어의 방들을 생성하고 관리함 (꾸미기전용 방 관리자)
    /// </summary>
    public class PlayerRoomsManager : SingletonMonoBase<PlayerRoomsManager>
    {
        private PlayerRoomsPresenter _playerRoomsPresenter;
        private static bool _lockAwake;

        [HideInInspector] public LinkedList<Room> rooms = new LinkedList<Room>();
        private float _roomDistance = 20.0f;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        /// <summary>
        /// Just for testing
        /// </summary>
        [Obsolete]
        public void SaveRooms()
        {
            PlayerRoomsData.instance.Items = rooms
                .Select(x => new RoomData() 
                { 
                    id = x.id.value, 
                    items = x.items
                        .Where(z => z != null)
                        .Select(y => new ItemData()
                                {
                                    id = y.id.value,
                                    position = y.transform.localPosition,
                                    rotation = y.transform.localRotation
                                }).ToList()
                }).ToList();
            PlayerRoomsData.instance.Save();
        }

        public void AddRoom(Room room)
        {
            if (_playerRoomsPresenter.addCommand.TryExecute(new DataModels.RoomData() 
            { 
                id = room.id.value, 
                items = new List<DataModels.ItemData>()
            }))
            {
                Debug.Log($"[PlayerRoomsManager] : Add room");
            }
            else
            {
                Debug.Log($"[PlayerRoomsManager] : Not enable to add room");
            }
        }

        public bool TryGetPrevRoom(Room room, out Room prev)
        {
            prev = null;

            if (room == null)
                return false;

            LinkedListNode<Room> tmp = rooms.Find(room);

            if (tmp.Previous != null)
            {
                tmp.Value.gameObject.SetActive(false);
                tmp.Previous.Value.gameObject.SetActive(true);
                prev = tmp.Previous.Value;
                return true;
            }

            return false;
        }

        public bool TryGetNextRoom(Room room, out Room next)
        {
            next = null;

            if (room == null)
                return false;

            LinkedListNode<Room> tmp = rooms.Find(room);

            if (tmp.Next != null)
            {
                tmp.Value.gameObject.SetActive(false);
                tmp.Next.Value.gameObject.SetActive(true);
                next = tmp.Next.Value;
                return true;
            }

            return false;
        }

        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override protected void Init()
        {
            base.Init();
            _playerRoomsPresenter = new PlayerRoomsPresenter();

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _playerRoomsPresenter.source != null);
                _playerRoomsPresenter.source.CollectionChanged += () =>
                {
                    // todo -> Refresh or Created / Delete room.
                };

                await UniTask.WaitUntil(() => RoomAssets.instance != null);
                for (int i = 0; i < _playerRoomsPresenter.source.Count; i++)
                {                    
                    Room room = await Room.CreateAsync(_playerRoomsPresenter.source[i], transform.position);
                    Debug.Log($"Room Created {room}");
                    rooms.AddLast(room);                    
                    await UniTask.WaitUntil(() => rooms.Count == i + 1);
                    room.gameObject.SetActive(false);
                }

                rooms.First().gameObject.SetActive(true);
                Player.instance.MoveTo(rooms.First());
            });
        }

    }
}
