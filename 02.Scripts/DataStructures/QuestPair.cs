using System;

namespace HTH.DataStructures
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 퀘스트 데이터 단위
    /// </summary>
    [Serializable]
    public struct QuestPair
    {
        public enum QuestStatus
        {
            None,
            WaitingAcception,
            Accepted,
            OnProgress,
        }
        public int id;
        public QuestStatus status;

        public QuestPair(int id, QuestStatus status)
        {
            this.id = id;
            this.status = status;
        }
    }
}
