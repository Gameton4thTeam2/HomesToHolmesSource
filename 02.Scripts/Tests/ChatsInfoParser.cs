using CsvHelper;
using CsvHelper.Configuration;
using HTH.DataModels;
using HTH.IDs;
using HTH.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 채팅정보 csv 파싱해서 ScriptableObject 생성해줌.
    /// </summary>
    public class ChatsInfoParser : MonoBehaviour
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
            TextAsset csvAsset = Resources.Load("Table_ChatsInfo") as TextAsset;
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                BadDataFound = null
            };

            List<ChatData> chats = new List<ChatData>();

            using (CsvReader csv = new CsvReader(new StreamReader("Assets/Resources/Table_ChatsInfo.csv"), config))
            {
                //Populate our list with all the raw records
                string[] record;

                int prevQuestID = -1;
                int questID = -1;
                int npcID = -1;
                string chat = String.Empty;
                int effectType = -1;
                float effectDuration = 01.0f;
                int illustType = -1;
                int pos = -1;
                while (csv.Parser.Read())
                {
                    //records.Add(csv.Parser.Record);
                    record = csv.Parser.Record;

                    questID = Convert.ToInt32(record[0]);

                    if (questID != prevQuestID )
                    {
                        ChatsInfo.CreateAsset($"Quest{prevQuestID}", new List<ChatData>(chats));
                        chats.Clear();
                        prevQuestID = questID;
                    }

                    npcID = Convert.ToInt32(record[1]);
                    chat = record[2];
                    effectType = Convert.ToInt32(record[3]);
                    effectDuration = float.Parse(record[4]);
                    illustType = Convert.ToInt32(record[5]);
                    pos = Convert.ToInt32(record[6]);

                    //string[] questIDGUIDs = AssetDatabase.FindAssets($"Quest{questID}", new string[] { "Assets/08.Data/QuestID" });
                    //if (questIDGUIDs.Length <= 0)
                    //{
                    //    Debug.LogWarning($"[ChatsInfoParser] : Failed to created {questID} data because Quest ID doesn't exist");
                    //    return;
                    //}

                    string[] npcIDGUIDs = AssetDatabase.FindAssets($"NPC_{npcID}", new string[] { "Assets/08.Data/NPCID" });
                    if (npcIDGUIDs.Length <= 0)
                    {
                        Debug.LogWarning($"[ChatsInfoParser] : Failed to created {questID} data because NPC ID doesn't exist");
                        break;
                    }

                    //AssetDatabase.LoadAssetAtPath<QuestID>(AssetDatabase.GUIDToAssetPath(questIDGUIDs[0]));


                    chats.Add(new ChatData()
                    {
                        npcID = AssetDatabase.LoadAssetAtPath<NPCID>(AssetDatabase.GUIDToAssetPath(npcIDGUIDs[0])),
                        chatEffect = new IllustEffect()
                        {
                            effectType = (IllustEffectType)effectType,
                            duration = effectDuration
                        },
                        illust = (IllustType)illustType,
                        illustPos = (Pos)pos,
                        chattingType = ChattingType.Chatting,
                        content = chat,
                    });
                }

                ChatsInfo.CreateAsset($"Quest{prevQuestID}", new List<ChatData>(chats));
                chats.Clear();
                prevQuestID = questID;
            }
        }
    }
}
#endif