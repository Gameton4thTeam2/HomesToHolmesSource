using HTH.IDs;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_09
    /// 설명    : 해쉬태그 참조 에셋
    /// </summary>
    public class HashtagAssets : MonoBehaviour
    {
        public static HashtagAssets instance
        {
            get
            {
                //if (_instance == null)
                //    _instance = Instantiate(Resources.Load<ItemAssets>("DataModels/ItemAssets"));
                return _instance;
            }
        }
        private static HashtagAssets _instance;
        [SerializeField] private List<Hashtag> _datas = new List<Hashtag>();
        public Hashtag this[int id] => _dataPairs[id];
        private Dictionary<int, Hashtag> _dataPairs = new Dictionary<int, Hashtag>();


        //===========================================================================
        //                             Private Methods
        //===========================================================================


        public void Initialize(List<Hashtag> datas)
        {
            _datas = datas;
            foreach (var data in _datas)
            {
                _dataPairs.Add(data.index, data);
            }
        }

        private void Awake()
        {
            _instance = this;

            if (_datas.Count <= 0)
                return;

            foreach (var data in _datas)
            {
                _dataPairs.Add(data.index, data);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}