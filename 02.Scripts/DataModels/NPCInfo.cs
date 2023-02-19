using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTH.IDs;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : NPC 정보 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "new NPC Info", menuName = "HomesToHolmes/Create NPCInfo")]
    public class NPCInfo : ScriptableObject
    {
        public NPCID id;
        new public string name;
        public string job;
        public string decription;
        public string address;
        public BuildingID buildilngID;
        public List<QuestID> quests;
    }
}