using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HTH.DataModels;
using UnityEngine.EventSystems;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 퀘스트 목록에 사용하는 슬롯. QuestInfo 로 값 세팅.
    /// </summary>
    public class QuestSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _address;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _npcIcon;
        private QuestInfo _questInfo;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_questInfo == null)
                return;

            QuestInfoUI.instance.Show(_questInfo);
        }

        public void SetUp(QuestInfo mainQuestInfo)
        {
            _questInfo = mainQuestInfo;
            if (mainQuestInfo != null)
            {
                _name.text = NPCAssets.instance[mainQuestInfo.npcId.value].name;
                _address.text = NPCAssets.instance[mainQuestInfo.npcId.value].address;
                _title.text = mainQuestInfo.title;
                _npcIcon.sprite = NPCIconAssets.instance[mainQuestInfo.npcId];
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