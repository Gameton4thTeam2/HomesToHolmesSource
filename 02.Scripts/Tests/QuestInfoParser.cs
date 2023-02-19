using CsvHelper;
using CsvHelper.Configuration;
using HTH.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
namespace HTH
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_02_12
    /// 설명    : 퀘스트정보 csv 파싱해서 ScriptableObject 생성해줌.
    /// </summary>
    public class QuestInfoParser : MonoBehaviour
    {
        [Serializable]
        private class IntList
        {
            public List<int> Items = new List<int>();
            public int this[int index] => Items[index];
            public IntList(IEnumerable<int> copy) => Items = new List<int>(copy);

            public int[] ToArray() => Items.ToArray();
        }


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Parser()
        {
            List<string[]> records = new List<string[]>();
            TextAsset csvAsset = Resources.Load("Table_ChatsInfo") as TextAsset;
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                BadDataFound = null
            };

            using (CsvReader csv = new CsvReader(new StreamReader("Assets/Resources/Table_QuestInfo.csv"), config))
            {
                string[] record;

                while (csv.Parser.Read())
                {
                    record = csv.Parser.Record; // 한줄씩 읽어서 문자열 배열로 저장

                    QuestInfo.CreateAssets(id: Convert.ToInt32(record[0]),
                                           npcId: record[1],
                                           title: record[2],
                                           description: record[3],
                                           budget: JsonUtility.FromJson<IntList>(record[4]).ToArray(),
                                           itemProvidedList: JsonUtility.FromJson<IntList>(record[5]).ToArray(),
                                           itemShoppingList: JsonUtility.FromJson<IntList>(record[6]).ToArray(),
                                           statsRequired: JsonUtility.FromJson<IntList>(record[7]).ToArray(),
                                           hashtagsRequired: JsonUtility.FromJson<IntList>(record[8]).ToArray(),
                                           colorRequired: record[9],
                                           rewardGold: JsonUtility.FromJson<IntList>(record[10]).ToArray(),
                                           rewardItems_RankS: JsonUtility.FromJson<IntList>(record[11]).ToArray(),
                                           rewardItems_RankA: JsonUtility.FromJson<IntList>(record[12]).ToArray(),
                                           rewardItems_RankB: JsonUtility.FromJson<IntList>(record[13]).ToArray(),
                                           rewardItems_RankC: JsonUtility.FromJson<IntList>(record[14]).ToArray(),
                                           rewardItems_RankD: JsonUtility.FromJson<IntList>(record[15]).ToArray(),
                                           rewardItems_RankE: JsonUtility.FromJson<IntList>(record[16]).ToArray()
                                           );
                }
            }
        }
    }
}
#endif