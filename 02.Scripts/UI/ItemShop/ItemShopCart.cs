using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HTH.DataModels;
using HTH.DataStructures;
using HTH.DataDependencySources;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 상점 UI
    /// </summary>
    public class ItemShopCart : MonoBehaviour
    {
        public bool isActivated;
        [SerializeField] private ItemShopUI _shop;
        [SerializeField] private ItemShopCartAdder _cartAdderPrefab;
        private Queue<ItemShopCartAdder> _adderPool = new Queue<ItemShopCartAdder>();
        private List<ItemShopCartAdder> _adders = new List<ItemShopCartAdder>();
        [SerializeField] private ItemShopCartPurchasePopUp _purchasePopUp;
        private InventoryPresenter _inventoryPresenter;
        [SerializeField] private GameObject _cartOnPanel;
        [SerializeField] private GameObject _cartOffPanel;
        [SerializeField] private Button _cartButton;
        [SerializeField] private Button _cancelCartButton;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Button _exitButton;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        /// <summary>
        /// 모든 아이템샵 슬롯에 장바구니 추가하는 버튼패널 붙이고 활성화
        /// </summary>
        public void ActiveCartAdders()
        {
            ItemShopCartAdder tmp;
            foreach (ItemShopSlot slot in _shop.GetSlots())
            {
                if (_adderPool.Count <= 0)
                {
                    tmp = Instantiate(_cartAdderPrefab, slot.transform);
                    _adders.Add(tmp);
                    _adderPool.Enqueue(tmp);
                }

                tmp = _adderPool.Dequeue();
                tmp.itemID = slot.itemInfo.id.value;
                tmp.transform.SetParent(slot.transform);
                tmp.transform.localPosition = new Vector3(0.0f, -30.0f, 0.0f);
                tmp.gameObject.SetActive(true);
            }
            Debug.Log("[ItemShopCart] : Activated ");
            isActivated = true;
        }

        /// <summary>
        /// 장바구니 추가버튼 비활성화
        /// </summary>
        public void DeactiveCartAdders()
        {
            _adderPool.Clear();

            foreach (ItemShopCartAdder adder in _adders)
            {
                adder.gameObject.SetActive(false);
                adder.Clear();
                _adderPool.Enqueue(adder);
            }
            isActivated = false;
        }

        /// <summary>
        /// 구매 비용 총합 차감 후 장바구니 목록을 인벤토리에 저장
        /// </summary>
        public void Purchase()
        {
            if (AssetData.instance.DecreaseGold(GetTotalPrice()))
            {
                foreach (var item in _adders.Select(x => new ItemPair(x.itemID, x.num)))
                {
                    if (item.num > 0)
                        _inventoryPresenter.addCommand.Execute(item);
                }
            }
            else
            {
                Debug.LogError("[ItemShopCart] : Failed to purchase. not enough gold");
            }
            DeactiveCartAdders();
        }

        /// <summary>
        /// 장바구니 아이템 구매 비용 총합
        /// </summary>
        public Gold GetTotalPrice()
        {
            Gold totalPrice = new Gold();
            foreach (ItemShopCartAdder adder in _adders)
            {
                totalPrice += ItemAssets.instance[adder.itemID].buyPrice * adder.num;
            }
            return totalPrice;
        }

        public void Clear()
        {
            foreach (ItemShopCartAdder adder in _adders)
            {
                adder.Clear();
            }
        }

        /// <summary>
        /// 구매창 팝업
        /// </summary>
        public void ShowPurchasePopUp()
        {
            _purchasePopUp.SetTotalPriceText(GetTotalPrice().GetSimplifiedString());
            _purchasePopUp.gameObject.SetActive(true);
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _inventoryPresenter = new InventoryPresenter();

            _cartButton.onClick.AddListener(() =>
            {
                ActiveCartAdders();
                _cartOffPanel.SetActive(false);
                _cartOnPanel.SetActive(true);
            });
            _cancelCartButton.onClick.AddListener(() =>
            {
                DeactiveCartAdders();
                _cartOnPanel.SetActive(false);
                _cartOffPanel.SetActive(true);
            });
            _purchaseButton.onClick.AddListener(() =>
            {
                ShowPurchasePopUp();
            });
            _exitButton.onClick.AddListener(() =>
            {
                DeactiveCartAdders();
                _cartOnPanel.SetActive(false);
                _cartOffPanel.SetActive(true);
                _shop.Hide();
            });
            _purchasePopUp.OnPurchaseButtonClick += () =>
            {
                Purchase();
                _purchasePopUp.gameObject.SetActive(false);
                _cartOnPanel.SetActive(false);
                _cartOffPanel.SetActive(true);
            };
            _purchasePopUp.OnCancelButtonClick += () =>
            {
                _purchasePopUp.gameObject.SetActive(false);
            };

            ActiveCartAdders();
            DeactiveCartAdders();
        }
    }
}