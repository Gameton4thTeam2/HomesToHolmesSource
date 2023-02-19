using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTH.DataModels;
using UnityEngine.EventSystems;
using HTH.DataDependencySources;
using System.Linq;
using System;
using Unity.VisualScripting;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using HTH.DataStructures;
using Random = UnityEngine.Random;
using HTH.GameSystems;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_02_09
    /// 설명    : 출석 이벤트를 위한 클래스
    /// 수정자  : 권병석
    /// 수정일  : 2023_02_13
    /// 설명    : 출석 그리드를 선택할 때 팝업창 추가
    /// </summary>
    public class AttendanceUI : UIMonoBehaviour<AttendanceUI>
    {
        /// <summary>
        /// 각 그리드마다 아이템 정보를 Json으로 받아와 리스트로 저장하기위해 사용하는 클래스
        /// </summary>
        public class AttendanceitemList
        {
            public List<int> items1x1;
            public List<int> items2x2;
            public List<int> items3x3;
        }

        [SerializeField] private Color _1x1Color;
        [SerializeField] private Color _2x2Color;
        [SerializeField] private Color _3x3Color;
        [SerializeField] private GameObject _1x1Panel;
        [SerializeField] private GameObject _2x2Panel;
        [SerializeField] private GameObject _3x3Panel;
        [SerializeField] private GameObject _previewPanel;
        [SerializeField] private Button _randomChange;
        [SerializeField] private TMP_Text _randomChangeCount;
        [SerializeField] private TMP_Text _randomChangePrice;
        [SerializeField] private Button _premiumRandomChange;
        [SerializeField] private Sprite _rewardIcon;

        private Image _previewImage;
        private Button _previewButton;

        private List<Button> _1x1Buttons = new List<Button>();
        private List<Image> _1x1Images = new List<Image>();
        private List<Image> _1x1ScrollViewImages = new List<Image>();

        private List<Button> _2x2Buttons = new List<Button>();
        private List<Image> _2x2Images = new List<Image>();
        private List<Image> _2x2ScrollViewImages = new List<Image>();

        private List<Button> _3x3Buttons = new List<Button>();
        private List<Image> _3x3Images = new List<Image>();
        private List<Image> _3x3ScrollViewImages = new List<Image>();

        private int[] _1x1SelectedGridList = Enumerable.Repeat<int>(0, 1).ToArray<int>();
        private int[] _2x2SelectedGridList = Enumerable.Repeat<int>(0, 4).ToArray<int>();
        private int[] _3x3SelectedGridList = Enumerable.Repeat<int>(0, 9).ToArray<int>();

        private AttendanceitemList _attendanceitemList;
        private FirebaseManager _firebase;
        private InventoryPresenter _inventoryPresenter;
        private bool _isFirstAttendance = true;
        private bool _isStartAttendance = false;
        private bool _isPossibleAttendance;
        private bool _isExistPreviewItem = false;
        private bool _isEndCheck = true;
        private bool _is1x1Item = true;
        private bool _is2x2Item = true;
        private bool _is3x3Item = true;
        private int _currentIndex;
        private int _currentRandomChangeCount = 5;
        private int _checkFinishCount = 0;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public override void Show()
        {
            FirebaseManager.instance.RefreshServerTime();
            base.Show();
            FinishAttendance();
        }

        public override void Hide()
        {
            base.Hide();
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            FirebaseManager.instance.RefreshServerTime();
            _1x1Panel.GetComponentsInChildren(_1x1Buttons);
            _2x2Panel.GetComponentsInChildren(_2x2Buttons);
            _3x3Panel.GetComponentsInChildren(_3x3Buttons);

            _previewImage = _previewPanel.transform.GetChild(0).GetComponent<Image>();
            _previewButton = _previewPanel.transform.GetChild(0).GetComponent<Button>();
            _randomChange.onClick.AddListener(RandomChageItem);
            _premiumRandomChange.onClick.AddListener(PremiumRandomChangeItem);

            _attendanceitemList = JsonUtility.FromJson<AttendanceitemList>(Resources.Load<TextAsset>("AttendanceItemData/AttendanceItem").ToString());
            _firebase = FirebaseManager.instance;
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => ItemAssets.instance != null);
                await UniTask.WaitUntil(() => InventoryData.instance != null);
                _inventoryPresenter = new InventoryPresenter();
                LoadAttendancePreviewDataFromDatabase();
                CheckExistSelectedAttendanceItem();
            });

            base.Init();
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        /// <summary>
        /// Init을 할 때 저장된 Preview 데이터가 있는지 확인하고 로드하는 함수
        /// </summary>
        private async void LoadAttendancePreviewDataFromDatabase()
        {
            if (User.nickName == "Guest")
                return;

            var gridSnapShot = await _firebase[1, "AttendancePreviewItemGrid"].GetValueAsync();
            if (gridSnapShot.Exists && gridSnapShot.Value.ConvertTo<int>() != -1)
            {
                _isExistPreviewItem = true;
                var indexSnapShot = await _firebase[1, "AttendancePreviewItemIndex"].GetValueAsync();
                var changeCountSnapShot = await _firebase[1, "AttendancePreviewChangeCount"].GetValueAsync();
                int grid = int.Parse(gridSnapShot.Value.ToString());
                int index = int.Parse(indexSnapShot.Value.ToString());
                int changeCount = changeCountSnapShot.Value.ConvertTo<int>();
                if (grid == 1)
                {
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items1x1[index]].icon;
                    _previewPanel.GetComponent<Image>().color = _1x1Color;
                    InitItemGridSize(index, true, false, false);
                    InitPreviewImageButton();
                }
                else if(grid == 2)
                {
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items2x2[index]].icon;
                    _previewPanel.GetComponent<Image>().color = _2x2Color;
                    InitItemGridSize(index, false, true, false);
                    InitPreviewImageButton();
                }
                else if(grid == 3)
                {
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items3x3[index]].icon;
                    _previewPanel.GetComponent<Image>().color = _3x3Color;
                    InitItemGridSize(index, false, false, true);
                    InitPreviewImageButton();
                }
                else
                {
                    throw new Exception("[AttendanceUI] : 저장된 Grid가 없습니다.");
                }
                _currentRandomChangeCount = changeCount;
                _randomChangeCount.text = _currentRandomChangeCount.ToString() + "/5";
                _randomChangePrice.text = (1200 - (200 * _currentRandomChangeCount)).ToString();
                if (_currentRandomChangeCount == 0)
                    _randomChangePrice.text = "0";
            }
            else
            {
                InitPreviewImageButton();
            }
        }

        /// <summary>
        /// 데이터베이스에서 시간을 가져와서 출석이 가능한지 체크하는 함수
        /// </summary>
        private async Task CheckPossibleAttendance()
        {
            if (User.nickName == "Guest")
                return;

            _isEndCheck = false;
            var clientSnapShot = await _firebase[1, "AttendanceLastTime"].GetValueAsync();
            if(clientSnapShot.Exists)
            {
                double clientTime = double.Parse(clientSnapShot.Value.ToString());
                DateTime clientDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
                DateTime clientKoreaTime = clientDateTime.AddMilliseconds(clientTime);

                var serverSnapShot = await _firebase[0, "ServerTime"].GetValueAsync();
                double serverTime = double.Parse(serverSnapShot.Value.ToString());
                DateTime serverDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
                DateTime serverKoreaTime = serverDateTime.AddMilliseconds(serverTime);

                Debug.Log($"{serverKoreaTime}, {clientKoreaTime}");
                if (serverKoreaTime.Day - clientKoreaTime.Day > 0)
                {
                    _isPossibleAttendance = true;
                    _isEndCheck = true;
                }
                else
                {
                    _isPossibleAttendance = false;
                    _isEndCheck = true;
                }
            }
            else
            {
                _isPossibleAttendance = true;
                _isEndCheck = true;
            }
        }

        /// <summary>
        /// 선택된 출석 아이템이 있는지 확인하고 있으면 로드하는 함수
        /// </summary>
        private async void CheckExistSelectedAttendanceItem()
        {
            if (User.nickName == "Guest")
                return;

            var snapShot = await _firebase[1, "AttendanceItemGrid"].GetValueAsync();
            if(snapShot.Exists)
            {
                if (int.Parse(snapShot.Value.ToString()) != -1)
                {
                    InitLoadAttendanceDataFromDatabase();
                }
                else
                    return;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 데이터베이스에 저장된 출석 정보를 받아와서 세팅해주는 함수
        /// </summary>
        /// <exception cref="Exception"></exception>
        private async void InitLoadAttendanceDataFromDatabase()
        {
            if (User.nickName == "Guest")
                return;

            var gridSnapShot = await _firebase[1, "AttendanceItemGrid"].GetValueAsync();
            var countSnapShot = await _firebase[1, "AttendanceItemCount"].GetValueAsync();
            var selectedSnapShot = await _firebase[1, "AttendanceItemGridSelected"].GetValueAsync();
            var indexSnapShot = await _firebase[1, "AttendanceItemIndex"].GetValueAsync();

            int grid = int.Parse(gridSnapShot.Value.ToString());
            int count = int.Parse(countSnapShot.Value.ToString());
            List<int> tmp = new List<int>();
            foreach (var item in selectedSnapShot.Children)
            {
                tmp.Add(item.Value.ConvertTo<int>());
            }
            int[] selected = tmp.ToArray();
            int index = int.Parse(indexSnapShot.Value.ToString());
            
            _isStartAttendance = false;

            if (grid == 1)
            {
                _previewPanel.SetActive(false);
                InitItemGridSize(index, true, false, false);
                _checkFinishCount = count;
                _1x1SelectedGridList = selected;
                _1x1Panel.SetActive(true);
                InitAttendanceButton();
                for (int i = 0; i < _1x1Images.Count; i++)
                {
                    _1x1Images[i].sprite = ItemAssets.instance[_attendanceitemList.items1x1[index]].icon;
                    _1x1Images[i].color = Color.black;
                }
                for (int i = 0; i < _1x1ScrollViewImages.Count; i++)
                {
                    _1x1ScrollViewImages[i].color = Color.white;
                }
                for (int i = 0; i < _1x1SelectedGridList.Length; i++)
                {
                    if (_1x1SelectedGridList[i] == 1)
                    {
                        _1x1Images[i].color = Color.white;
                        _1x1ScrollViewImages[i].color = Color.clear;
                        _1x1Buttons[i].onClick.RemoveAllListeners();
                    }
                    else
                        continue;
                }
            }
            else if(grid == 2)
            {
                _previewPanel.SetActive(false);
                InitItemGridSize(index, false, true, false);
                _checkFinishCount = count;
                _2x2SelectedGridList = selected;
                _2x2Panel.SetActive(true);
                InitAttendanceButton();
                for (int i = 0; i < _2x2Images.Count; i++)
                {
                    _2x2Images[i].sprite = ItemAssets.instance[_attendanceitemList.items2x2[index]].icon; ;
                    _2x2Images[i].color = Color.black;
                }
                for (int i = 0; i < _2x2ScrollViewImages.Count; i++)
                {
                    _2x2ScrollViewImages[i].color = Color.white;
                }
                for (int i = 0; i < _2x2SelectedGridList.Length; i++)
                {
                    if (_2x2SelectedGridList[i] == 1)
                    {
                        _2x2Images[i].color = Color.white;
                        _2x2ScrollViewImages[i].color = Color.clear;
                        _2x2Buttons[i].onClick.RemoveAllListeners();
                    }
                    else
                        continue;
                }
            }
            else if(grid == 3)
            {
                _previewPanel.SetActive(false);
                InitItemGridSize(index, false, false, true);
                _checkFinishCount = count;
                _3x3SelectedGridList = selected;
                _3x3Panel.SetActive(true);
                InitAttendanceButton();
                for (int i = 0; i < _3x3Images.Count; i++)
                {
                    _3x3Images[i].sprite = ItemAssets.instance[_attendanceitemList.items3x3[index]].icon; ;
                    _3x3Images[i].color = Color.black;
                }
                for (int i = 0; i < _3x3ScrollViewImages.Count; i++)
                {
                    _3x3ScrollViewImages[i].color = Color.white;
                }
                for (int i = 0; i < _3x3SelectedGridList.Length; i++)
                {
                    if (_3x3SelectedGridList[i] == 1)
                    {
                        _3x3Images[i].color = Color.white;
                        _3x3ScrollViewImages[i].color = Color.clear;
                        _3x3Buttons[i].onClick.RemoveAllListeners();
                    }
                    else
                        continue;
                }
            }
            else
            {
                throw new Exception("[AttendanceUI] : 로드할 데이터가 없습니다.");
            }
        }

        /// <summary>
        /// 랜덤 아이템 변경 함수
        /// </summary>
        private void RandomChageItem()
        {
            if (User.nickName == "Guest")
                return;

            if (!_isStartAttendance)
                return;

            if(_currentRandomChangeCount <= 0)
            {
                WarningWindowPopUpUI.instance.Show("[랜덤교체 횟수 초과]", "가구를 완성하거나 프리미엄 보상교체를 눌러주세요.");
                return;
            }
            if(!_isFirstAttendance)
            {
                Gold changePrice = new Gold(int.Parse(_randomChangePrice.text), 0, 0, 0);
                _currentRandomChangeCount--;
                AssetData.instance.DecreaseGold(changePrice);
            }
            int probability = Random.Range(0, 100);
            if(probability > 80) // Low
            {
                int id = Random.Range(0, _attendanceitemList.items1x1.Count);
                _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items1x1[id]].icon;
                _previewPanel.GetComponent<Image>().color = _1x1Color;
                InitItemGridSize(id, true, false, false);
                _firebase.SavePreviewAttendanceDataOnDatabase(1, id, _currentRandomChangeCount);
            }
            else if(probability > 30) // Proper
            {
                int id = Random.Range(0, _attendanceitemList.items2x2.Count);
                _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items2x2[id]].icon;
                _previewPanel.GetComponent<Image>().color = _2x2Color;
                InitItemGridSize(id, false, true, false);
                _firebase.SavePreviewAttendanceDataOnDatabase(2, id, _currentRandomChangeCount);
            }
            else // High
            {
                int id = Random.Range(0, _attendanceitemList.items3x3.Count);
                _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items3x3[id]].icon;
                _previewPanel.GetComponent<Image>().color = _3x3Color;
                InitItemGridSize(id, false, false, true);
                _firebase.SavePreviewAttendanceDataOnDatabase(3, id, _currentRandomChangeCount);
            }
            _randomChangeCount.text = _currentRandomChangeCount.ToString() + "/5";
            _randomChangePrice.text = (1200 - (200 * _currentRandomChangeCount)).ToString();
            if (_currentRandomChangeCount == 0)
                _randomChangePrice.text = "0";
        }

        /// <summary>
        /// 프리미엄 랜덤 아이템 변경 함수
        /// todo => 광고 적용
        /// </summary>
        private void PremiumRandomChangeItem()
        {
            if (User.nickName == "Guest")
                return;

            if (!_isStartAttendance)
                return;

            _currentRandomChangeCount = 5;
            if (_is1x1Item)
            {
                int probability = Random.Range(0, 100);
                if (probability > 80) // Low
                {
                    int id = Random.Range(0, _attendanceitemList.items1x1.Count);
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items1x1[id]].icon;
                    _previewPanel.GetComponent<Image>().color = _1x1Color;
                    InitItemGridSize(id, true, false, false);
                    _firebase.SavePreviewAttendanceDataOnDatabase(1, id, _currentRandomChangeCount);
                }
                else if (probability > 30) // Proper
                {
                    int id = Random.Range(0, _attendanceitemList.items2x2.Count);
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items2x2[id]].icon;
                    _previewPanel.GetComponent<Image>().color = _2x2Color;
                    InitItemGridSize(id, false, true, false);
                    _firebase.SavePreviewAttendanceDataOnDatabase(2, id, _currentRandomChangeCount);
                }
                else // High
                {
                    int id = Random.Range(0, _attendanceitemList.items3x3.Count);
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items3x3[id]].icon;
                    _previewPanel.GetComponent<Image>().color = _3x3Color;
                    InitItemGridSize(id, false, false, true);
                    _firebase.SavePreviewAttendanceDataOnDatabase(3, id, _currentRandomChangeCount);
                }
            }
            else if (_is2x2Item)
            {
                int probability = Random.Range(0, 80);
                if (probability > 30) // Proper
                {
                    int id = Random.Range(0, _attendanceitemList.items2x2.Count);
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items2x2[id]].icon;
                    _previewPanel.GetComponent<Image>().color = _2x2Color;
                    InitItemGridSize(id, false, true, false);
                    _firebase.SavePreviewAttendanceDataOnDatabase(2, id, _currentRandomChangeCount);
                }
                else // High
                {
                    int id = Random.Range(0, _attendanceitemList.items3x3.Count);
                    _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items3x3[id]].icon;
                    _previewPanel.GetComponent<Image>().color = _3x3Color;
                    InitItemGridSize(id, false, false, true);
                    _firebase.SavePreviewAttendanceDataOnDatabase(3, id, _currentRandomChangeCount);
                }
            }
            else if(_is3x3Item)
            {
                int id = Random.Range(0, _attendanceitemList.items3x3.Count);
                _previewImage.sprite = ItemAssets.instance[_attendanceitemList.items3x3[id]].icon;
                _previewPanel.GetComponent<Image>().color = _3x3Color;
                InitItemGridSize(id, false, false, true);
                _firebase.SavePreviewAttendanceDataOnDatabase(4, id, _currentRandomChangeCount);
            }
            _randomChangeCount.text = _currentRandomChangeCount.ToString() + "/5";
            _randomChangePrice.text = (1200 - (200 * _currentRandomChangeCount)).ToString();
            if (_currentRandomChangeCount == 0)
                _randomChangePrice.text = "0";
        }
        
        /// <summary>
        /// 현재 아이템 인덱스 및 아이템 그리드 사이즈를 초기화하는 함수
        /// </summary>
        private void InitItemGridSize(int index, bool is1x1, bool is2x2, bool is3x3)
        {
            _currentIndex = index;
            _is1x1Item = is1x1;
            _is2x2Item = is2x2;
            _is3x3Item = is3x3;
        }

        /// <summary>
        /// 처음 출석을 시작할 때 랜덤으로 아이템을 띄우기위해 사용하는 함수
        /// </summary>
        private void FirstStartAttendace()
        {
            _isStartAttendance = true;
            RandomChageItem();
        }

        /// <summary>
        /// Preview에서 아이템을 선택해 출석을 시작하기위해 사용하는 함수
        /// </summary>
        private void StartAttendace()
        {
            if (User.nickName == "Guest")
                return;

            _isStartAttendance = false;
            _previewPanel.SetActive(false);
            if (_is1x1Item)
            {
                _1x1Panel.SetActive(true);
                for (int i = 0; i < _1x1Images.Count; i++)
                {
                    _1x1Images[i].sprite = _previewImage.sprite;
                    _1x1Images[i].color = Color.black;
                }
                for (int i = 0; i < _1x1ScrollViewImages.Count; i++)
                {
                    _1x1ScrollViewImages[i].color = Color.white;
                }
                _firebase.RegisterAndInitAttendanceOnDatabase(1, 0, _currentIndex, _1x1SelectedGridList);
            }
            else if (_is2x2Item)
            {
                _2x2Panel.SetActive(true);
                for (int i = 0; i < _2x2Images.Count; i++)
                {
                    _2x2Images[i].sprite = _previewImage.sprite;
                    _2x2Images[i].color = Color.black;
                }
                for (int i = 0; i < _2x2ScrollViewImages.Count; i++)
                {
                    _2x2ScrollViewImages[i].color = Color.white;
                }
                _firebase.RegisterAndInitAttendanceOnDatabase(2, 0, _currentIndex, _2x2SelectedGridList);
            }
            else if (_is3x3Item)
            {
                _3x3Panel.SetActive(true);
                for (int i = 0; i < _3x3Images.Count; i++)
                {
                    _3x3Images[i].sprite = _previewImage.sprite;
                    _3x3Images[i].color = Color.black;
                }
                for (int i = 0; i < _3x3ScrollViewImages.Count; i++)
                {
                    _3x3ScrollViewImages[i].color = Color.white;
                }
                _firebase.RegisterAndInitAttendanceOnDatabase(3, 0, _currentIndex, _3x3SelectedGridList);
            }
            else
            {
                throw new System.Exception("[AttendanceUI] : 선택한 가구가 없음");
            }
        }

        /// <summary>
        /// 각 그리드 버튼마다 부여하는 함수
        /// </summary>
        private async void OnClickAttendanceButton()
        {
            if (User.nickName == "Guest")
                return;

            if (!_isEndCheck)
                return;

            await CheckPossibleAttendance();

            GameObject go = EventSystem.current.currentSelectedGameObject;

            if (_isPossibleAttendance)
            {
                ConfirmWindowPopUpUI.instance.Show(() =>
                {
                    go.GetComponent<Image>().color = Color.clear;
                    go.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>().color = Color.white;
                    go.GetComponent<Button>().onClick.RemoveAllListeners();
                    _checkFinishCount++;
                    int index = int.Parse(go.name.Split('_')[1]);
                    Debug.Log($"{index}");
                    SaveSelectedGridListFromDatabase(index);
                    _firebase.SaveAttendanceTimeFromDatabase();
                    FinishAttendance();
                    ConfirmWindowPopUpUI.instance.Hide();
                    _isPossibleAttendance = false;
                }, "출석하시겠습니까?");
            }
            else
            {
                WarningWindowPopUpUI.instance.Show("알림", "오늘은 출석을 완료하였습니다.");
                return;
            }
        }

        /// <summary>
        /// 그리드 버튼을 클릭했을 때 선택된 그리드 리스트를 데이터베이스에 저장하는 함수
        /// </summary>
        private void SaveSelectedGridListFromDatabase(int index)
        {
            if(_is1x1Item)
            {
                _1x1SelectedGridList[index] = 1;
                _firebase.SaveSelectedItemGridFromDatabase(_checkFinishCount, _1x1SelectedGridList);
            }
            else if(_is2x2Item)
            {
                _2x2SelectedGridList[index] = 1;
                _firebase.SaveSelectedItemGridFromDatabase(_checkFinishCount, _2x2SelectedGridList);
            }
            else if(_is3x3Item)
            {
                _3x3SelectedGridList[index] = 1;
                _firebase.SaveSelectedItemGridFromDatabase(_checkFinishCount, _3x3SelectedGridList);
            }
            else
            {
                throw new Exception("[AttendanceUI] : 아이템 그리드 정보가 없습니다.");
            }
        }

        /// <summary>
        /// 출석이 종료됐을 때 아이템을 부여하고 출석 정보를 초기화하는 함수
        /// </summary>
        private void FinishAttendance()
        {
            if(_is1x1Item)
            {
                if(_checkFinishCount >= 1)
                {
                    RewardWindowPopUpUI.instance.Show(() =>
                    {
                        Debug.Log("1x1가구를 하나 받았다...!!!");
                        _1x1Panel.SetActive(false);
                        _isExistPreviewItem = false;
                        InitPreviewImageButton();
                        _checkFinishCount = 0;
                        _1x1SelectedGridList = Enumerable.Repeat<int>(0, 1).ToArray<int>();
                        _firebase.RegisterAndInitAttendanceOnDatabase(-1, 0, -1, _1x1SelectedGridList);
                        _firebase.SavePreviewAttendanceDataOnDatabase(-1, -1, 5);
                        _inventoryPresenter.addCommand.Execute(new ItemPair(ItemAssets.instance[_attendanceitemList.items1x1[_currentIndex]].id.value, 1));
                        RewardWindowPopUpUI.instance.Hide();
                    }, "가구를 받았다...", _previewImage, _1x1Color, 1);
                }
            }
            else if(_is2x2Item)
            {
                if (_checkFinishCount >= 4)
                {
                    RewardWindowPopUpUI.instance.Show(() =>
                    {
                        Debug.Log("4x4가구를 하나 받았다...!!!");
                        _2x2Panel.SetActive(false);
                        _isExistPreviewItem = false;
                        InitPreviewImageButton();
                        _checkFinishCount = 0;
                        _2x2SelectedGridList = Enumerable.Repeat<int>(0, 4).ToArray<int>();
                        _firebase.RegisterAndInitAttendanceOnDatabase(-1, 0, -1, _2x2SelectedGridList);
                        _firebase.SavePreviewAttendanceDataOnDatabase(-1, -1, 5);
                        _inventoryPresenter.addCommand.Execute(new ItemPair(ItemAssets.instance[_attendanceitemList.items2x2[_currentIndex]].id.value, 1));
                        RewardWindowPopUpUI.instance.Hide();
                    }, "가구를 받았다...", _previewImage, _2x2Color, 1);
                }
            }
            else if (_is3x3Item)
            {
                if (_checkFinishCount >= 9)
                {
                    RewardWindowPopUpUI.instance.Show(() =>
                    {
                        Debug.Log("5x5가구를 하나 받았다...!!!");
                        _3x3Panel.SetActive(false);
                        _isExistPreviewItem = false;
                        InitPreviewImageButton();
                        _checkFinishCount = 0;
                        _3x3SelectedGridList = Enumerable.Repeat<int>(0, 9).ToArray<int>();
                        _firebase.RegisterAndInitAttendanceOnDatabase(-1, 0, -1, _3x3SelectedGridList);
                        _firebase.SavePreviewAttendanceDataOnDatabase(-1, -1, 5);
                        _inventoryPresenter.addCommand.Execute(new ItemPair(ItemAssets.instance[_attendanceitemList.items3x3[_currentIndex]].id.value, 1));
                        RewardWindowPopUpUI.instance.Hide();
                    }, "가구를 받았다...", _previewImage, _3x3Color, 1);
                }
            }
            else
            {
                throw new System.Exception("[AttendanceUI] : 받을 가구가 없음");
            }
        }

        /// <summary>
        /// Preview 버튼을 초기화하는 함수
        /// </summary>
        private void InitPreviewImageButton()
        {
            if (!_previewPanel.activeSelf)
                _previewPanel.SetActive(true);
            
            _isFirstAttendance = false;
            _isStartAttendance = true;
            if (!_isExistPreviewItem)
            {
                _isFirstAttendance = true;
                _isStartAttendance = false;
                _previewImage.sprite = _rewardIcon;
                _previewPanel.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            }
            _previewButton.onClick.AddListener(() =>
            {
                if (_isFirstAttendance)
                {
                    ConfirmWindowPopUpUI.instance.Show(() =>
                    {
                        FirstStartAttendace();
                        _isFirstAttendance = false;
                        ConfirmWindowPopUpUI.instance.Hide();
                    }, "출석할랑가?");
                }
                else
                {
                    ConfirmWindowPopUpUI.instance.Show(() =>
                    {
                        _checkFinishCount = 0;
                        InitAttendanceButton();
                        StartAttendace();
                        _isStartAttendance = false;
                        ConfirmWindowPopUpUI.instance.Hide();
                    }, "이 가구로 진행할까요?");
                }
            });
        }

        /// <summary>
        /// 출석 아이템이 선택됐을 때 아이템에 대한 그리드로 그리드 버튼들을 초기화하는 함수
        /// </summary>
        private void InitAttendanceButton()
        {
            if (_is1x1Item)
            {
                _1x1Images.Clear();
                _1x1ScrollViewImages.Clear();
                for (int i = 0; i < _1x1Buttons.Count; i++)
                {
                    _1x1Images.Add(_1x1Buttons[i].gameObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>());
                    _1x1ScrollViewImages.Add(_1x1Buttons[i].GetComponent<Image>());
                    _1x1Buttons[i].onClick.AddListener(() =>
                    {
                        OnClickAttendanceButton();
                    });
                }
            }
            if(_is2x2Item)
            {
                _2x2Images.Clear();
                _2x2ScrollViewImages.Clear();
                for (int i = 0; i < _2x2Buttons.Count; i++)
                {
                    _2x2Images.Add(_2x2Buttons[i].gameObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>());
                    _2x2ScrollViewImages.Add(_2x2Buttons[i].GetComponent<Image>());
                    _2x2Buttons[i].onClick.AddListener(() =>
                    {
                        OnClickAttendanceButton();
                    });
                }
            }
            if(_is3x3Item)
            {
                _3x3Images.Clear();
                _3x3ScrollViewImages.Clear();
                for (int i = 0; i < _3x3Buttons.Count; i++)
                {
                    _3x3Images.Add(_3x3Buttons[i].gameObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>());
                    _3x3ScrollViewImages.Add(_3x3Buttons[i].GetComponent<Image>());
                    _3x3Buttons[i].onClick.AddListener(() =>
                    {
                        OnClickAttendanceButton();
                    });
                }
            }
        }
    }
}