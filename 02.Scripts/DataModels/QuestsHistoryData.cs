using HTH.DataStructures;
using HTH.GameSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_12
    /// 설명    : 메인퀘스트 이력 데이터모델. 추가한 아이템 삭제 불가능하며 데이터 변경에 대해 통지 하지않음.
    /// </summary>
    [Serializable]
    public class QuestsHistoryData : CollectionDataModelBase<QuestHistoryPair, QuestsHistoryData>
    {
        private static string _path;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public QuestsHistoryData() { }

        public Rank GetRank(int questID) => Items.Find(q => q.questID == questID).rank;

        public void AddItem(QuestHistoryPair questHistoryPair)
        {
            int index = Items.FindIndex(item => item.questID == questHistoryPair.questID);
            if (index >= 0)
                base.Set(index, new QuestHistoryPair(Items[index].questID, questHistoryPair.rank));
            else
                base.Add(questHistoryPair);
        }
        


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override public void Load()
        {
            if (User.isloggedIn == false)
            {
                throw new Exception("[MainQuestsHistoryData] : 유저 정보 찾을 수 없음.");
            }

            _path = User.dataRepoDirectory + "/MainQuestsHistoryData.json";

            QuestsHistoryData tmpData;
            if (System.IO.File.Exists(_path) == false)
            {
                tmpData = new QuestsHistoryData();
                tmpData.Items = new List<QuestHistoryPair>();
                System.IO.File.WriteAllText(_path, JsonUtility.ToJson(tmpData));
            }
            else
            {
                tmpData = JsonUtility.FromJson<QuestsHistoryData>(System.IO.File.ReadAllText(_path));
            }

            Items = tmpData.Items;
        }

        override public void Save()
        {
            System.IO.File.WriteAllText(_path, JsonUtility.ToJson(instance));
        }
        

        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            base.Init();
            Load();
        }
    }
}