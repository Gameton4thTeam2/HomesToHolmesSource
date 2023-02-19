using HTH.WorldElements;
using HTH.DataStructures;
using HTH.DataDependencySources;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using HTH.GameSystems;
using HTH.InputHandlers;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : WorldMap 의 아이템 선택시 아이템을 조작하기위한 UI
    /// </summary>
    public class ItemUI : UIMonoBehaviour<ItemUI>
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Button _rotate;
        [SerializeField] private Button _remove;
        private RectTransform _rect;
        private Camera _cam;        
        private LayerMask _targetLayer;
        private Item _selected;
        private InventoryPresenter _presenter;
        private CanvasGroup _canvasGroup;

        // UI Raycast event
        [HideInInspector] public GraphicRaycaster _raycaster;
        [HideInInspector] public PointerEventData _pointerEventData;
        [HideInInspector] public EventSystem _eventSystem;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void ShowUnmanaged(Item item)
        {
            _selected = item;
            base.ShowUnmanaged();
        }

        public override void Show()
        {
            throw new System.Exception("Not implemented");
        }

        public override void HideUnmanaged()
        {
            _rect.position = Vector3.one * 5000.0f;
            _selected = null;
            base.HideUnmanaged();
        }

        public override void Hide()
        {
            throw new System.Exception("Not implemented");
        }

        public void CheckOutsideDown()
        {
            // UI 캐스팅 확인
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            _raycaster.Raycast(_pointerEventData, results);

            CanvasRenderer tmpCanvasRenderer = null;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent(out tmpCanvasRenderer))
                {
                    return;
                }
            }

            // 월드내 선택된 아이템 외 다른곳 클릭 확인
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _targetLayer) &&
                hit.transform != _selected.transform)
            {
                //HideUnmanaged();
            }
            HideUnmanaged();

        }

        public void OnRotate()
        {
            _selected.transform.Rotate(Vector3.up * 90, Space.Self);
            
        }

        public void OnRemove()
        {
            if (PlayerRoomsManager.instance.rooms.Contains(Player.instance.currentRoom))
            {
                int id = _selected.id.value;
                if (_presenter.addCommand.CanExecute(new ItemPair(id, 1)))
                {
                    Destroy(_selected.gameObject);
                    _presenter.addCommand.Execute(new ItemPair(id, 1));
                    HideUnmanaged();
                }
            }
            else
            {
                QuestManager.instance.items.Remove(_selected);
                Destroy(_selected.gameObject);
                HideUnmanaged();
            }
        }

        public void SetTransparency(float a)
        {
            _canvasGroup.alpha = a;
        }

        //===========================================================================
        //                             Protected Methods
        //===========================================================================
        
        override protected void Init()
        {
            _presenter = new InventoryPresenter();
            _raycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = FindObjectOfType<EventSystem>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _rect = transform.GetChild(0).GetComponent<RectTransform>();
            _targetLayer = 1 << LayerMask.NameToLayer("Item");
            _cam = CameraController.instance.worldCam;
            _rotate.onClick.AddListener(OnRotate);
            _remove.onClick.AddListener(OnRemove);
            base.Init();
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Update()
        {
            if (_selected == null)
                return;

            Vector2 screenPoint = _cam.WorldToScreenPoint(_selected.transform.position);
            _rect.position = screenPoint + _offset;

            if (Input.GetMouseButtonDown(0))
            {
                CheckOutsideDown();
            }
        }
    }
}