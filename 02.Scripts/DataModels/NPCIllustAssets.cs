using HTH.IDs;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 퀘스트 목록에 사용하는 슬롯. QuestInfo 로 값 세팅.
    /// </summary>
    public class NPCIllustAssets : MonoBehaviour
    {
        private static NPCIllustAssets _instance;
        public static NPCIllustAssets instance
        {
            get
            {
                //if (_instance == null)
                //    _instance = Instantiate(Resources.Load<NPCIllustAssets>("DataModels/NPCIllustAssets"));
                return _instance;
            }
        }
        [SerializeField] private List<UKeyValuePair<NPCID, List<UKeyValuePair<IllustType, Sprite>>>> _illusts;
        private Dictionary<NPCID, Dictionary<IllustType, Sprite>> _illustDictionary = new Dictionary<NPCID, Dictionary<IllustType, Sprite>>();
        public Dictionary<IllustType, Sprite> this[NPCID npcID] => _illustDictionary[npcID];


        private void Awake()
        {
            _instance = this;

            foreach (var pairs in _illusts)
            {
                _illustDictionary.Add(pairs.key, new Dictionary<IllustType, Sprite>());
                foreach (var pair in pairs.value)
                {
                    _illustDictionary[pairs.key].Add(pair.key, pair.value);
                }
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}