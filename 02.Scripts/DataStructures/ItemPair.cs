using System;

namespace HTH.DataStructures
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 ID 와 갯수 쌍
    /// </summary>
    [Serializable]
    public struct ItemPair : IComparable<ItemPair>
    {
        public static ItemPair empty => new ItemPair(-1, -1);
        public int id;
        public int num;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public ItemPair(int id, int num)
        {
            this.id = id;
            this.num = num;
        }

        public static bool operator ==(ItemPair op1, ItemPair op2)
        {
            return op1.id == op2.id;// && op1.num == op2.num;
        }

        public static bool operator !=(ItemPair op1, ItemPair op2)
        {
            return !(op1 == op2);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemPair pair &&
                   id == pair.id;// &&
                   //num == pair.num;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, num);
        }

        public int CompareTo(ItemPair other)
        {
            if (id < other.id)
                return -1;
            else if (id == other.id)
                return 0;
            else
                return -1;
        }
    }
}