using HTH;
using System;

namespace HTH.DataStructures
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 퀘스트 내역 데이터 단위
    /// </summary>
    [Serializable]
    public struct QuestHistoryPair : IComparable<QuestHistoryPair>
    {
        public static QuestHistoryPair empty => new QuestHistoryPair(-1, Rank.None);
        public int questID;
        public Rank rank;

        public QuestHistoryPair(int questID, Rank rank)
        {
            this.questID = questID;
            this.rank = rank;
        }

        public int CompareTo(QuestHistoryPair other)
        {
            return rank.CompareTo(other.rank);
        }
    }
}