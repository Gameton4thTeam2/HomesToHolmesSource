using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HTH.UI
{
    /// <summary>
    /// 작성자 : 조영민
    /// 작성일 : 2023/01/23
    /// 설명 : 로딩화면. Resources 에서 불러와야하므로 GameObject 의 이름이 클래스이름과 동일해야하고 Resources Root 디렉토리에 둬야함.
    /// </summary>
    public class LoadingUI : UIMonoBehaviour<LoadingUI>
    {
        [SerializeField] private RectTransform _progressSliderBG;
        [SerializeField] private RectTransform _progressSliderBar;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private List<GameObject> _onLoadingObjects;
        [SerializeField] private float _onLoadingObjectSwitchingDelay = 0.2f;
        private float _onLoadingObjectsIndex;
        private float _timeMark;
        private volatile IntPtr _pProgress;
        private CancellationTokenSource _cts;


        //============================================================================
        //                           Public Methods  
        //============================================================================

        public void ShowUnmanged(IntPtr pProgress)
        {
            _pProgress = pProgress;
            _progressSliderBar.anchorMax = new Vector2(GetProgress(), 0.5f);
            _progress.text = $"{(Math.Truncate(GetProgress() * 100.0f) / 100.0f)} %";
            base.ShowUnmanaged(99);

            _cts = new CancellationTokenSource();
            RefreshProgress().Forget();
        }

        /// <summary>
        /// 로딩화면 띄우고 비동기 씬 로딩시 호출
        /// </summary>
        /// <param name="pProgress"></param>
        /// <param name="sceneName"></param>
        public void Show(IntPtr pProgress, string sceneName)
        {
        }

        public override void Hide()
        {
            base.Hide();
            _cts.Cancel();
        }


        //============================================================================
        //                          Protected Methods   
        //============================================================================

        protected override void Init()
        {
            base.Init();
            if (instance)
                DontDestroyOnLoad(instance.gameObject);
        }


        //============================================================================
        //                          Private Methods 
        //============================================================================

        private async UniTaskVoid RefreshProgress()
        {
            if (_pProgress != null)
            {
                while (IsProgressFinished(1.0f) == false)
                {
                    _progressSliderBar.anchorMax = new Vector2(GetProgress(), 0.5f);
                    _progress.text = $"{(Math.Truncate(GetProgress() * 10000.0f) / 100.0f)} %";

                    using (CancellationTokenSource linkedCTS =
                              CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, this.GetCancellationTokenOnDestroy()))
                    {
                        await UniTask.Yield(linkedCTS.Token);
                    }
                }

                HideUnmanaged();
            }
        }

        private unsafe bool IsProgressFinished(float standard)
        {
            return (*(float*)(_pProgress.ToPointer())) >= standard;
        }

        private unsafe float GetProgress()
        {
            return *(float*)(_pProgress.ToPointer());
        }

        private void SwitchOnLoadingObjects()
        {
            for (int i = 0; i < _onLoadingObjects.Count; i++)
            {
                if (i == _onLoadingObjectsIndex)
                    _onLoadingObjects[i].SetActive(true);
                else
                    _onLoadingObjects[i].SetActive(false);

                _onLoadingObjectsIndex = _onLoadingObjectsIndex < _onLoadingObjects.Count - 1 ? _onLoadingObjectsIndex + 1 : 0;
            }
        }

        private void Update()
        {
            if (Time.deltaTime - _timeMark > _onLoadingObjectSwitchingDelay)
            {
                SwitchOnLoadingObjects();
                _timeMark = Time.deltaTime;
            }
        }
    }
}