using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HTH.DataModels;
using UnityEngine.EventSystems;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 정보 UI. 
    /// </summary>
    public class ItemInfoUI : UIMonoBehaviour<ItemInfoUI>
    {        
        public ItemInfo itemInfo
        {
            set
            {
                if (value == null)
                    return;

                _name.text = value.name;
                _description.text = value.description;
                _image.sprite = value.icon;
            }
        }
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _image;
        [SerializeField] private Button _close;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================
                
        public void Show(ItemInfo itemInfo, Vector2 pos)
        {
            this.itemInfo = itemInfo;
            transform.position = pos;
            base.Show();
        }

        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        protected override void Init()
        {
            _close.onClick.AddListener(Hide);
            base.Init();
        }

    }
}