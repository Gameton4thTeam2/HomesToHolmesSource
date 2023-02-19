using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HTH.DataStructures;
using HTH.DataModels;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 보상 정보 표시용 슬롯. 골드는 기준 금액에 따라 스프라이트 바뀜
    /// </summary>
    public class RewardPreviewSlot : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _num;
        [SerializeField] private Sprite _goldSprite0;
        [SerializeField] private Gold _goldStandard1;
        [SerializeField] private Sprite _goldSprite1;
        [SerializeField] private Gold _goldStandard2;
        [SerializeField] private Sprite _goldSprite2;
        [SerializeField] private Gold _goldStandard3;
        [SerializeField] private Sprite _goldSprite3;
        [SerializeField] private Gold _goldStandard4;
        [SerializeField] private Sprite _goldSprite4;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void SetUp(ItemPair pair)
        {
            _image.sprite = ItemAssets.instance[pair.id].icon;
            _num.text = pair.num.ToString();
        }

        public void SetUp(Sprite icon, int num)
        {
            _image.sprite = icon;
            _num.text = num.ToString();
        }

        public void SetUp(Gold gold)
        {
            if (gold >= _goldStandard4)
                _image.sprite = _goldSprite4;
            else if (gold >= _goldStandard3)
                _image.sprite = _goldSprite3;
            else if (gold >= _goldStandard2)
                _image.sprite = _goldSprite2;
            else if (gold >= _goldStandard1)
                _image.sprite = _goldSprite1;
            else
                _image.sprite = _goldSprite0;

            _num.text = gold.GetSimplifiedString();
        }
    }
}