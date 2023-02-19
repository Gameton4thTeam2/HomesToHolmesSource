using HTH.GameSystems;
using System;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_17
    /// 설명    : 진행중 퀘스트 목록 데이터 모델
    /// </summary>
    [Serializable]
    public class QuestsInProgressData : CollectionDataModelBase<int, QuestsInProgressData>
    {
        private string _path;

        
        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public QuestsInProgressData() { }

        public override void Load()
        {
            if (User.isloggedIn == false)
            {
                throw new Exception("[QuestsInProgressData] : 유저 정보 찾을 수 없음.");
            }

            _path = User.dataRepoDirectory + "/QuestsInProgressData.json";

            QuestsInProgressData tmpData;
            if (System.IO.File.Exists(_path) == false)
            {
                tmpData = new QuestsInProgressData();
                tmpData.Items = new System.Collections.Generic.List<int>();
                System.IO.File.WriteAllText(_path, JsonUtility.ToJson(tmpData));
            }
            else
            {
                tmpData = JsonUtility.FromJson<QuestsInProgressData>(System.IO.File.ReadAllText(_path));
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