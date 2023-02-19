using HTH.IDs;
using HTH.UI;
using System;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : NPC 와의 대화 데이터 단위
    /// 수정자  : 권병석
    /// 수정일  : 2023_02_01
    /// 설명    : 채팅 타입 추가 => 채팅모드, 생각모드
    /// </summary>
    [Serializable]
    public class ChatData
    {
        public NPCID npcID;
        public IllustEffect chatEffect;
        public IllustType illust;
        public Pos illustPos;
        public ChattingType chattingType;
        public string content;
    }
}