using CsvHelper;
using CsvHelper.Configuration;
using HTH.DataModels;
using HTH.IDs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 아이템정보 csv 파싱해서 ScriptableObject 생성해줌.
    /// </summary>
    public class ItemInfoParser : MonoBehaviour
    {
        [Serializable]
        private class IntList
        {
            public List<int> Items = new List<int>();
            public int this[int index] => Items[index];
            public IntList(IEnumerable<int> copy) => Items = new List<int>(copy);

            public int[] ToArray() => Items.ToArray();
        }

        public void Parse()
        {
            List<string[]> records = new List<string[]>();
            TextAsset csvAsset = Resources.Load("Table_ItemInfo") as TextAsset;
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                BadDataFound = null
            };
            using (CsvReader csv = new CsvReader(new StreamReader("Assets/Tables/Table_ItemInfo.csv"),config))
            {
                string[] record;
                while (csv.Parser.Read())
                {
                    record = csv.Parser.Record;

                    ItemInfo.CreateAsset(id: Convert.ToInt32(record[0]),
                                         name: record[1],
                                         description: record[2],
                                         optionFlags: Convert.ToInt32(record[3]),
                                         hashtags: JsonUtility.FromJson<IntList>(record[4]).ToArray(),
                                         stats: JsonUtility.FromJson<IntList>(record[5]).ToArray(),
                                         buyPrice: JsonUtility.FromJson<IntList>(record[6]).ToArray(),
                                         sellPrice: JsonUtility.FromJson<IntList>(record[7]).ToArray(),
                                         rarity: Convert.ToInt32(record[8]));
                }
            }            
        }        

        public void CreatHashtags(int total)
        {
            Hashtag.CreateAssets(total);
        }
    }
}
#endif