using System;
using UnityEngine;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : UIManager 에 의해 관리되는 UI 기본 컴포넌트. 
    /// 수정자  : 권병석
    /// 수정일  : 2023_02_01
    /// 설명    : UIMonoBehaviour 클래스를 종속받는 UI가 UIManager에 의해 Register이 강제 되지 않도록 virtual로 수정
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class UIMonoBehaviour<T> : SingletonMonoBase<T>, IUI
        where T : UIMonoBehaviour<T>
    {
        protected UIManager _manager;
        protected virtual UIManager manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = UIManager.instance;
                    _manager.Register(this);
                }
                return _manager;
            }
        }

        public int sortingOrder 
        {
            get => canvas.sortingOrder;
            set => canvas.sortingOrder = value;
        }
        public Canvas canvas
        {
            get
            {
                if (_canvas == null)
                    _canvas = GetComponent<Canvas>();
                return _canvas;
            }
        }
        private Canvas _canvas;
        

        public event Action OnShow;
        public event Action OnHide;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public virtual void Show()
        {
            manager.Push(this);
            gameObject.SetActive(true);            
            OnShow?.Invoke();
        }

        public virtual void Hide()
        {
            if (manager.Pop(this))
            {
                gameObject.SetActive(false);
                OnHide?.Invoke();
            }
        }

        public virtual void ShowUnmanaged()
        {
            canvas.sortingOrder = manager.uisShown.Count + 1;
            gameObject.SetActive(true);
            OnShow?.Invoke();
        }

        public virtual void ShowUnmanaged(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
            gameObject.SetActive(true);
            OnShow?.Invoke();
        }

        public virtual void HideUnmanaged()
        {
            gameObject.SetActive(false);
            OnHide?.Invoke();
        }

        public virtual void ShowWithoutNotification()
        {
            gameObject.SetActive(true);
            manager.Push(this);
        }

        public virtual void HideWithoutNotification()
        {
            if (manager.Pop(this))
                gameObject.SetActive(false);
        }

        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            base.Init();
            HideUnmanaged();
        }


        //===========================================================================
        //                             Prrivate Methods
        //===========================================================================

        private void OnDestroy()
        {
            manager.Remove(this);
        }
    }
}