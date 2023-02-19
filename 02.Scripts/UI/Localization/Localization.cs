using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

namespace HTH.UI
{
    /// <summary>
    /// 작성자      : 권병석
    /// 작성일      : 2023_01_19
    /// 설명        : Localization 데이터를 csv에서 파싱하여 저장시키는 클래스
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_12
    /// 설명        : 안드로이드에서 csv 파싱이 불가능했던 오류 수정[2023_01_26, 권병석]
    ///               Localization의 각 데이터의 종류마다 분리해서 테이블에 저장하도록 수정, 테이블을 참조할 수 있는 인덱서 추가[2023_02_09, 권병석]
    ///               Hashtag 데이터 참조 오류 수정
    /// </summary>
    public class Localization
    {
        public event Action OnLanguageChange;
        public static Localization instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Localization();
                }
                return _instance;
            }
        }
        private static Localization _instance;
        public int language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
                OnLanguageChange?.Invoke();
            }
        }
        private int _language;
        private Dictionary<string, Dictionary<string, List<string>>> _table = new Dictionary<string, Dictionary<string, List<string>>>();
        List<string> _fileNameList = new List<string>
        {
            "ItemName",
            "ItemDescription",
            "ItemRarity",
            "Stat",
            "Hashtag",
            "NPCName",
            "NPCJob",
            "NPCAddress",
            "QuestTitle",
            "QuestDescription",
            "QuestChat",
            "QuestColor",
            "SystemMessage"
        };


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public Localization()
        {
            Init();
        }

        public string this[string tableName, string indexString]
        {
            get
            {
                if(tableName == "ITEM_NAME")
                {
                    return _table["ItemName"][indexString][_language];
                }
                else if(tableName == "ITEM_DES")
                {
                    return _table["ItemDescription"][indexString][_language];
                }
                else if(tableName == "ITEM_RARITY")
                {
                    return _table["ItemRarity"][indexString][_language];
                }
                else if(tableName == "STAT_NAME")
                {
                    return _table["Stat"][indexString][_language];
                }
                else if(tableName == "#HASHTAG_NAME")
                {
                    indexString = indexString.Replace("#", "");
                    return "#" + _table["Hashtag"][indexString][_language];
                }
                else if(tableName == "NPC_NAME")
                {
                    return _table["NPCName"][indexString][_language];
                }
                else if(tableName == "NPC_JOB")
                {
                    return _table["NPCJob"][indexString][_language];
                }
                else if(tableName == "NPC_ADDRESS")
                {
                    return _table["NPCAddress"][indexString][_language];
                }
                else if(tableName == "QUEST_TITLE")
                {
                    return _table["QuestTitle"][indexString][_language];
                }
                else if(tableName == "QUEST_DES")
                {
                    return _table["QuestDescription"][indexString][_language];
                }
                else if(tableName == "QUEST_CHAT" || tableName == "TUTORIAL_QUEST" || tableName == "INTRO_CHAT")
                {
                    return _table["QuestChat"][indexString][_language];
                }
                else if(tableName == "QUEST_COLOR")
                {
                    return _table["QuestColor"][indexString][_language];
                }
                else if(tableName == "SYSTEM_MESSAGE")
                {
                    return _table["SystemMessage"][indexString][_language];
                }    
                else
                {
                    return null;
                }
            }
        }

        public Dictionary<string, List<string>> ReadTable(string path)
        {
            var list = new Dictionary<string, List<string>>();
            TextAsset sourceFile = Resources.Load<TextAsset>(path);
            StringReader sr = new StringReader(sourceFile.text);
            var Language = (sr.ReadLine()).Split(',');
            while (sr.Peek() > -1)
            {
                bool hasQM = false; // QM = Quotation Mark (")
                bool startQM = false;
                int index = 0;
                string dataString = sr.ReadLine();
                var data = dataString.Split(',');
                List<string> tmp = new List<string>();
                string[] datas = new string[3];
                foreach (var item in data)
                {
                    if(startQM)
                    {
                        tmp.Add(item);
                        if (item.EndsWith('"'))
                        {
                            startQM = false;
                            datas[index] = MergeSentence(tmp);
                            tmp.Clear();
                            index++;
                            hasQM = true;
                        }
                        continue;
                    }
                    if (item.StartsWith('"'))
                    {
                        startQM = true;
                        tmp.Add(item);
                    }
                    else
                    {
                        datas[index] = item;
                        index++;
                    }
                }
                var tmpList = new List<string>();
                tmpList.Add(datas[1]); // datas[1] : Korea
                tmpList.Add(datas[2]); // datas[2] : English
                list.Add(datas[0], tmpList); // data[0] : Index
            }
            return list;
        }

        public static string MergeSentence(List<string> str)
        {
            string tmp = string.Empty;
            bool isMergeStart = false;
            StringBuilder stringbuilder = new StringBuilder();
            stringbuilder.Clear();
            foreach (var item in str)
            {
                if (item.StartsWith('"') && !isMergeStart)
                {
                    isMergeStart = true;
                    stringbuilder.Append(item.Substring(1)).Append(",");
                }
                else if (isMergeStart && !item.EndsWith('"'))
                {
                    stringbuilder.Append(item).Append(",");
                }
                else if (item.EndsWith('"'))
                {
                    stringbuilder.Append(item.Substring(0, item.Length - 1));
                    isMergeStart = false;
                    tmp = stringbuilder.ToString();
                }
                else
                {
                    tmp = item;
                }
            }
            return tmp;
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Init()
        {
            foreach (var name in _fileNameList)
            {
                string path = $"LocalizationData/{name}";
                Dictionary<string, List<string>> tmp = ReadTable(path);
                _table.Add(name, tmp);
                Debug.Log($"[Localization] : {path} 로드 완료");
            }
            language = PlayerPrefs.GetInt("Language");
        }
    }
}