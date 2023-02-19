using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 상점 장바구니 추가 UI
    /// </summary>
    public class ItemShopCartAdder : MonoBehaviour
    {
        private const int MAX_NUM = 99;
        public int itemID;       
        public int num
        {
            get
            {
                return _num;
            }
            set
            {
                if (value < 0 ||
                    value > MAX_NUM)
                    return;

                _num = value;
                _numText.text = _num.ToString();
            }
        }
        private int _num;
        [SerializeField] private Button _decreaseButton;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private TMP_Text _numText;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Clear()
        {
            itemID = -1;
            num = 0;
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _decreaseButton.onClick.AddListener(() => num--);
            _increaseButton.onClick.AddListener(() => num++);
        }
    }
}