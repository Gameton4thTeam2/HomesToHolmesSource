using HTH.DataModels;
using HTH.DataDependencySources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_17
    /// 설명    : 의뢰함의 슬롯 클릭시 나타나는 의뢰 세부내용 UI
    /// </summary>
    public class QuestBoxDetailsUI : UIMonoBehaviour<QuestBoxDetailsUI>
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _address;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _npcIcon;
        [SerializeField] private Button _accept;
        private QuestsPresenter _questPresenter;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void Show(QuestInfo questInfo)
        {
            if (questInfo != null)
            {
                _name.text = NPCAssets.instance[questInfo.npcId.value].name;
                _address.text = NPCAssets.instance[questInfo.npcId.value].address;
                _description.text = questInfo.description;
                _npcIcon.sprite = NPCIconAssets.instance[questInfo.npcId];
                _accept.gameObject.SetActive(_questPresenter.pendingSource.Contains(questInfo.id.value));
                if (_accept.gameObject.activeSelf)
                {
                    _accept.onClick.AddListener(() =>
                    {
                        ConfirmWindowPopUpUI.instance.Show(() =>
                        {
                            _questPresenter.acceptPendingCommand.Execute(questInfo.id.value);
                            QuestBoxUI.instance.Refresh();
                            ConfirmWindowPopUpUI.instance.Hide();
                            Hide();
                        },
                        "수락하시겠습니까?"
                        );
                    });
                }
                base.Show();
            }
            else
            {
                _name.text = string.Empty;
                _address.text = string.Empty;
                _description.text = string.Empty;
                _npcIcon.sprite = null;
                _accept.onClick.RemoveAllListeners();
                base.Hide();
            }
        }

        protected override void Init()
        {
            _questPresenter = new QuestsPresenter();
            base.Init();
        }
    }
}