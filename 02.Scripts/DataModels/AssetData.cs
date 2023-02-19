using System;
using UnityEngine;
using HTH.DataStructures;
using HTH.GameSystems;

/// <summary>
/// 작성자  : 조영민
/// 작성일  : 2023_01_09
/// 설명    : 유저의 재화(자산) 데이터모델
/// </summary>
namespace HTH.DataModels
{
    public class AssetData
    {
        private static AssetData _instance;
        public static AssetData instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();
                return _instance;
            }
        }
        private static string _path;
        public Gold gold;
        public event Action<Gold> GoldChanged;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void IncreaseGold(Gold amount)
        {
            if (this.gold >= Gold.max)
                return;

            this.gold += amount;
            Save();
            GoldChanged?.Invoke(this.gold);
        }

        public bool DecreaseGold(Gold amount)
        {
            if (this.gold < amount ||
                this.gold < Gold.min)
                return false;

            this.gold -= amount;
            Save();
            GoldChanged?.Invoke(this.gold);
            return true;
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private static AssetData Load()
        {
            if (User.isloggedIn == false)
            {
                throw new Exception("[AssetData] : 유저 정보 찾을 수 없음.");
            }

            _path = User.dataRepoDirectory + "/AssetData.json";

            AssetData tmpData;
            if (System.IO.File.Exists(_path) == false)
            {
                tmpData = new AssetData();
                tmpData.gold = new Gold() { tsp0 = 1000000 };
                System.IO.File.WriteAllText(_path, JsonUtility.ToJson(tmpData));
            }
            else
            {
                tmpData = JsonUtility.FromJson<AssetData>(System.IO.File.ReadAllText(_path));
            }

            return tmpData;
        }

        private static void Save()
        {
            System.IO.File.WriteAllText(_path, JsonUtility.ToJson(instance));
        }
    }
}
