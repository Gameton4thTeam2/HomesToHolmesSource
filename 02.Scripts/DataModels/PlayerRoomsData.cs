using HTH.GameSystems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_13
    /// 설명    : 플레이어의 방 데이터
    /// </summary>
    public class PlayerRoomsData : CollectionDataModelBase<RoomData, PlayerRoomsData>
    {
        private string _path;


        //===========================================================================
        //                             Public Methods
        //===========================================================================


        public PlayerRoomsData() { }

        override public void Load()
        {
            if (User.isloggedIn == false)
            {
                throw new Exception("[PlayerRoomsData] : 유저 정보 찾을 수 없음.");
            }

            _path = User.dataRepoDirectory + "/PlayerRoomsData.json";

            PlayerRoomsData tmpData;
            if (System.IO.File.Exists(_path) == false)
            {
                tmpData = new PlayerRoomsData();
                tmpData.Items.Add(new RoomData() { id = 1, items = new List<ItemData>() });
                tmpData.Items.Add(new RoomData() { id = 2, items = new List<ItemData>() });
                tmpData.Items.Add(new RoomData() { id = 3, items = new List<ItemData>() });
                System.IO.File.WriteAllText(_path, JsonUtility.ToJson(tmpData));
            }
            else
            {
                tmpData = JsonUtility.FromJson<PlayerRoomsData>(System.IO.File.ReadAllText(_path));
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