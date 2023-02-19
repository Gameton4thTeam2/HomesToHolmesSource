using HTH.DataStructures;
using HTH.GameSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_17
    /// 설명    : 수락한 퀘스트 목록 데이터
    /// </summary>
    [Serializable]
    public class QuestsAcceptedData : CollectionDataModelBase<int, QuestsAcceptedData>
    {
        private string _path;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        override public void Load()
        {
            if (User.isloggedIn == false)
            {
                throw new Exception("[QuestsAcceptedData] : 유저 정보 찾을 수 없음.");
            }

            _path = User.dataRepoDirectory + "/QuestsAcceptedData.json";

            QuestsAcceptedData tmpData;
            if (System.IO.File.Exists(_path) == false)
            {
                tmpData = new QuestsAcceptedData();
                tmpData.Items = new List<int>();
                System.IO.File.WriteAllText(_path, JsonUtility.ToJson(tmpData));
            }
            else
            {
                tmpData = JsonUtility.FromJson<QuestsAcceptedData>(System.IO.File.ReadAllText(_path));
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