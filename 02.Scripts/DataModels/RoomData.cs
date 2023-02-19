using System;
using System.Collections.Generic;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 방 데이터 단위
    /// </summary>
    [Serializable]
    public class RoomData : IComparable<RoomData>
    {
        public int id;
        public List<ItemData> items;

        public int CompareTo(RoomData other)
        {
            if (items.Count < other.items.Count)
                return -1;
            else if (items.Count < other.items.Count)
                return 0;
            else
                return -1;
        }
    }
}