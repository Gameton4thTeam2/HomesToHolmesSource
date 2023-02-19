using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_01_12
    /// 설명    : 플레이어 룸에서 메뉴 버튼을 클릭했을경우 표시되는 UI
    ///           메인 메뉴 버튼을 클릭했을때 메뉴 버튼들이 애니메이션처럼 On/Off 됨
    /// 수정자  : 조영민
    /// 수정일  : 2023_01_23
    /// 설명    : Show / Hide Coroutine 관련 체크 수정 및 Awake lock 설정
    /// </summary>
    public class MenuUI : UIMonoBehaviour<MenuUI>
    {
        [SerializeField] private GameObject _menuPanel;
        private List<RectTransform> _menuPositionList = new List<RectTransform>();
        
        private bool _showCoroutineOn;
        private bool _hideCoroutineOn;
        private Coroutine _coroutine;

        [SerializeField] private Button _outSide;
        [SerializeField] private Button _openQuestBox;
        [SerializeField] private Button _openSettings;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        new public void ShowUnmanaged()
        {
            base.ShowUnmanaged();

            if (_showCoroutineOn)
                return;
            
            // 코루틴은 실제로 호출하고나서 그다음 프레임부터 돌아가기때문에 On/Off 를 bool 로 체크하기위해서는 
            // 코루틴 호출 전에 bool 을 켜줘야하고 코루틴 함수 내에서 끝날때 bool 값을 false 해주는것이 좋아여
            _showCoroutineOn = true;
            _coroutine = StartCoroutine(E_ShowUnmanaged());
        }

        new public void HideUnmanaged()
        {
            if (_hideCoroutineOn)
                return;
            
            _hideCoroutineOn = true;
            _coroutine = StartCoroutine(E_HideUnmanaged());
        }

        public void Toggle()
        {
            if (_showCoroutineOn || 
                _hideCoroutineOn)
                return;

            if (gameObject.activeSelf)
                HideUnmanaged();
            else
                ShowUnmanaged();                
        }

        //===============================================================================================
        //                                  Protected Methods
        //===============================================================================================

        override protected void Init()
        {
            if (_menuPositionList.Count == 0)
            {
                for (int i = 0; i < _menuPanel.transform.childCount; i++)
                {
                    _menuPositionList.Add(_menuPanel.transform.GetChild(i).GetComponent<RectTransform>());
                }
            }

            _outSide.onClick.AddListener(Toggle);
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestBoxUI.instance != null &&
                                              SettingsUI.instance != null);
                _openQuestBox.onClick.AddListener(() =>
                {
                    HideUnmanaged();
                    QuestBoxUI.instance.Show();
                });
                _openSettings.onClick.AddListener(SettingsUI.instance.Show);
            });

            base.HideUnmanaged();
        }


        //===============================================================================================
        //                                  Private Methods
        //===============================================================================================

        IEnumerator E_ShowUnmanaged()
        {
            int i = 1;
            while (_menuPositionList[_menuPositionList.Count - 1].anchoredPosition3D.y > (-1 * (Constants.MAIN_MENU_BOX_HEIGHT + Constants.MAIN_MENU_PADDING) * 3) + Constants.MAIN_MENU_FIRST_START_Y)
            {
                if (i > _menuPositionList.Count - 1)
                    i = 1;
                for (int j = i; j < _menuPositionList.Count; j++)
                {
                    _menuPositionList[j].anchoredPosition3D += (Vector3.down * (Constants.MAIN_MENU_BOX_HEIGHT + Constants.MAIN_MENU_PADDING) * Constants.MAIN_MENU_ANIMATION_SPEED);
                }
                i++;
                yield return null;
            }
            _showCoroutineOn = false;
            _coroutine = null;
        }

        IEnumerator E_HideUnmanaged()
        {
            int i = _menuPositionList.Count - 1;
            while (_menuPositionList[_menuPositionList.Count - 1].anchoredPosition3D.y < Constants.MAIN_MENU_FIRST_START_Y)
            {
                if (i < 0)
                    i = _menuPositionList.Count - 1;
                for (int j = i; j > 0; j--)
                {
                    if (_menuPositionList[j].anchoredPosition3D.y >= Constants.MAIN_MENU_FIRST_START_Y)
                        continue;
                    _menuPositionList[j].anchoredPosition3D += (Vector3.up * (Constants.MAIN_MENU_BOX_HEIGHT + Constants.MAIN_MENU_PADDING) * Constants.MAIN_MENU_ANIMATION_SPEED * j);
                }
                i--;
                yield return null;
            }
            _hideCoroutineOn = false;
            _coroutine = null;
            base.HideUnmanaged();
        }
    }
}
