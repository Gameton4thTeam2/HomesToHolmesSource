using System.Collections.Generic;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : QuestInfo 에셋을 가져다 쓰기위한 클래스
    /// </summary>
    public class QuestAssets : MonoBehaviour
    {
        private static QuestAssets _instance;
        public static QuestAssets instance
        {
            get
            {
                //if (_instance == null)
                //    _instance = Instantiate(Resources.Load<QuestAssets>("DataModels/QuestAssets"));
                return _instance;
            }
        }

        [SerializeField] private List<QuestInfo> _questInfos = new List<QuestInfo>();
        private Dictionary<int, QuestInfo> _questInfosDictionary = new Dictionary<int, QuestInfo>();
        public QuestInfo this[int id] => _questInfosDictionary[id];

        private void Awake()
        {
            _instance = this;

            foreach (var questInfo in _questInfos)
            {
                _questInfosDictionary.Add(questInfo.id.value, questInfo);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}