using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 정보/ 아이템 프리팹을 참조하기위한 모델
    /// </summary>
    public class ItemAssets : MonoBehaviour
    {
        public static ItemAssets instance
        {
            get
            {
                //if (_instance == null)
                //    _instance = Instantiate(Resources.Load<ItemAssets>("DataModels/ItemAssets"));
                return _instance;
            }
        }
        private static ItemAssets _instance;
        [SerializeField] private List<ItemInfo> _datas = new List<ItemInfo>();
        public ItemInfo this[int id] => _dataPairs[id];
        private Dictionary<int, ItemInfo> _dataPairs = new Dictionary<int, ItemInfo>();


        //===========================================================================
        //                             Private Methods
        //===========================================================================


        public void Initialize(List<ItemInfo> datas)
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