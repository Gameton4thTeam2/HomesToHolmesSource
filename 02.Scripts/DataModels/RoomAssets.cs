using HTH.WorldElements;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 방 프리팹 참조용 데이터모델
    /// </summary>
    public class RoomAssets : MonoBehaviour
    {
        public static RoomAssets instance
        {
            get
            {
                //if (_instance == null)
                //    _instance = Instantiate(Resources.Load<RoomAssets>("DataModels/RoomAssets"));
                return _instance;
            }
        }
        private static RoomAssets _instance;
        public Room this[int id] => _roomPairs[id];
        [SerializeField] private List<Room> _rooms = new List<Room>();
        private Dictionary<int, Room> _roomPairs = new Dictionary<int, Room>();


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _instance = this;

            foreach (var room in _rooms)
            {
                _roomPairs.Add(room.id.value, room);
            }

            DontDestroyOnLoad(gameObject);
        }
    }

}
