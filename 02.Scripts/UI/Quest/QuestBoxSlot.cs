using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HTH.DataModels;
using HTH.DataDependencySources;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 퀘스트 목록에 사용하는 슬롯. QuestInfo 로 값 세팅.
    /// </summary>
    public class QuestBoxSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _address;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _npcIcon;
        [SerializeField] private Button _details;
        [SerializeField] private GameObject _accepted;
        private QuestInfo _questInfo;        


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void SetUp(QuestInfo questInfo, bool accepted)
        {
            _details.onClick.RemoveAllListeners();

            _questInfo = questInfo;
            if (questInfo != null)
            {
                _name.text = NPCAssets.instance[questInfo.npcId.value].name;
                _address.text = NPCAssets.instance[questInfo.npcId.value].address;
                _title.text = questInfo.title;
                _npcIcon.sprite = NPCIconAssets.instance[questInfo.npcId];
                _details.onClick.AddListener(() =>
                {
                    QuestBoxDetailsUI.instance.Show(_questInfo);
                });
                _accepted.SetActive(accepted);
            }
            else
            {
                _name.text = string.Empty;
                _address.text = string.Empty;
                _title.text = string.Empty;
                _npcIcon.sprite = null;
            }
        }
    }
}