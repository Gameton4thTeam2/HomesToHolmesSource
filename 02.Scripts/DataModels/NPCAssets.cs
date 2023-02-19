using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_13
    /// 설명    : NPC 정보를 참조하기위한 모델
    /// </summary>
    public class NPCAssets : MonoBehaviour
    {
        public static NPCAssets instance
        {
            get
            {
                //if (_instance == null)
                //    _instance = Instantiate(Resources.Load<NPCAssets>("DataModels/NPCAssets"));
                return _instance;
            }
        }
        private static NPCAssets _instance;
        [SerializeField] private List<NPCInfo> _datas;
        public NPCInfo this[int id] => _dataPairs[id];
        private Dictionary<int, NPCInfo> _dataPairs = new Dictionary<int, NPCInfo>();
        public IEnumerable<NPCInfo> npcInfos => _datas;

        //===========================================================================
        //                             Private Methods
        //===========================================================================


        public void Initialize(List<NPCInfo> datas)
        {
            _datas = datas;
            foreach (var data in _datas)
            {
                _dataPairs.Add(data.id.value, data);
            }
        }

        private void Awake()
        {
            _instance = this;

            if (_datas.Count <= 0)
                return;

            foreach (var data in _datas)
            {
                _dataPairs.Add(data.id.value, data);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}