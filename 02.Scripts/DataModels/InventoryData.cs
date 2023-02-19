using System;
using System.Collections.Generic;
using UnityEngine;
using HTH.DataStructures;
using HTH.GameSystems;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 유저의 인벤토리 데이터모델
    /// </summary>
    [Serializable]
    public class InventoryData : CollectionDataModelBase<ItemPair, InventoryData>
    {
        private string _path;
        

        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public InventoryData() { }

        public override void Add(ItemPair item)
        {
            int index = Items.FindIndex(x => x.id == item.id);
            if (index >= 0)
            {
                base.Set(index, new ItemPair(item.id, Items[index].num + item.num));
            }
            else
            {
                base.Add(item);
            }
        }

        public override bool Remove(ItemPair item)
        {
            bool isRemoved = false;
            int index = Items.FindIndex(x => x.id == item.id);
            if (index >= 0)
            {
                // 갯수 차감
                if (Items[index].num > item.num)
                {
                    base.Set(index, new ItemPair(item.id, Items[index].num - item.num));
                    isRemoved = true;
                }
                // 제거
                else if (Items[index].num == item.num)
                {
                    base.RemoveAt(index);                    
                    isRemoved = true;
                }
                else
                {
                    throw new InvalidOperationException($"[InventoryData] : Failed to remove item. not enough numbers");
                }
            }
            return isRemoved;
        }

        public bool TryGetItem(int itemID, ref ItemPair item)
        {
            item = Items.Find(x => x.id == itemID);
            return (item != default(ItemPair) &&
                    item != ItemPair.empty);
        }

        public override void Load()
        {
            if (User.isloggedIn == false)
            {
                throw new Exception("[InventoryData] : 유저 정보 찾을 수 없음.");
            }

            _path = User.dataRepoDirectory + "/InventoryData.json";

            InventoryData tmpData;
            if (System.IO.File.Exists(_path) == false)
            {
                tmpData = new InventoryData();
                tmpData.Items = new List<ItemPair>();
                //tmpData.Items.Add(new ItemPair(1001, 1)); // just for testing
                //tmpData.Items.Add(new ItemPair(3012, 1)); // just for testing
                //tmpData.Items.Add(new ItemPair(7010, 1)); // just for testing
                //tmpData.Items.Add(new ItemPair(8003, 1)); // just for testing
                System.IO.File.WriteAllText(_path, JsonUtility.ToJson(tmpData));
            }
            else
            {
                tmpData = JsonUtility.FromJson<InventoryData>(System.IO.File.ReadAllText(_path));
            }

            Items = tmpData.Items;
        }

        public override void Save()
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
