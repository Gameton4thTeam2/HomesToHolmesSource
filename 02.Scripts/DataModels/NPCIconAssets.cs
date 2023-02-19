using HTH.IDs;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : NPC 아이콘 에셋을 가져다쓰기위한 클래스
    /// </summary>
    public class NPCIconAssets : MonoBehaviour
    {
        private static NPCIconAssets _instance;
        public static NPCIconAssets instance
        {
            get
            {
                //if (_instance == null)
                //    _instance = Instantiate(Resources.Load<NPCIconAssets>("DataModels/NPCIconAssets"));
                return _instance;
            }
        }

        [SerializeField] private List<UKeyValuePair<NPCID, Sprite>> _icons;
        private Dictionary<NPCID, Sprite> _iconDictionary = new Dictionary<NPCID, Sprite>();
        public Sprite this[NPCID npcID] => _iconDictionary[npcID];


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _instance = this;

            foreach (var pairs in _icons)
            {
                _iconDictionary.Add(pairs.key, pairs.value);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}