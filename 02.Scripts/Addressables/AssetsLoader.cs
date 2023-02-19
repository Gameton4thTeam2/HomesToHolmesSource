using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using HTH.GameSystems;
using HTH.UI;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HTH.Assets
{
    /// <summary>
    /// 작성자 : 조영민
    /// 작성일 : 2023/01/23
    /// 설명 :     
    /// Addressable 을 사용해서 비동기로 필요 에셋들을 비동기로 로드하는 클래스.
    /// 로드해야할 참조에셋들이 모두 로드되면 isLoaded 가 true 가 되니, 이 프로퍼티가 true 반환된 후에 시스템플로우 진행해야함.
    /// </summary>
    public class AssetsLoader : SingletonMonoBase<AssetsLoader>
    {
        public bool isLoaded => _progressCounter >= assetsToLoad.Length + assetsToInstantiate.Length;

        public AssetReference[] assetsToLoad;
        public AssetReference[] assetsToInstantiate;
        private bool _isReady;
        private int _progressCounter;
        private IntPtr _pProgress;


        //============================================================================
        //                          Public Methods  
        //============================================================================

        /// <summary>
        /// 게임시작전 로드되어야 하는 에셋들 모두 로드
        /// </summary>
        public void LoadAllAssets()
        {
            Debug.Log("[AssetsLoader] : 에셋 로드 시작");

            ReallocProgress(); // Progress pointer 재할당

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _isReady);
                await UniTask.WaitUntil(() => GameManager.instance != null);
                await UniTask.WaitUntil(() => LoadingUI.instance != null);

                LoadingUI.instance.ShowUnmanged(_pProgress);
                
                for (int i = 0; i < assetsToLoad.Length; i++)
                {
                    assetsToLoad[i].LoadAssetAsync<GameObject>().Completed += (obj) =>
                    {   
                        Debug.Log($"[AssetsLoader] : {obj.Result} 로드완료");
                        Interlocked.Increment(ref _progressCounter);
                        RefreshProgress();
                    };
                }

                for (int i = 0; i < assetsToInstantiate.Length; i++)
                {
                    Addressables.InstantiateAsync(assetsToInstantiate[i]).Completed += (obj) =>
                    {
                        Debug.Log($"[AssetsLoader] : {obj.Result} 생성 완료");
                        Interlocked.Increment(ref _progressCounter);
                        RefreshProgress();
                    };
                }

                await UniTask.WaitUntil(() => _progressCounter >= (assetsToLoad.Length + assetsToInstantiate.Length));
                await UniTask.WaitUntil(() => GameManager.instance.current == GameManager.State.InGame);
                await UniTask.Delay(1000);
                RefreshProgress();
            });
        }

        protected override void Init()
        {
            base.Init();
            Addressables.InitializeAsync().Completed += (obj) => _isReady = true;
            DontDestroyOnLoad(gameObject);
        }


        //============================================================================
        //                            Private Methods 
        //============================================================================

        private unsafe void RefreshProgress()
        {
            *((float*)_pProgress.ToPointer()) 
                = (float)(_progressCounter) / (assetsToLoad.Length + assetsToInstantiate.Length);
            Debug.Log($"리소스 로드...  {_progressCounter} / {assetsToLoad.Length + assetsToInstantiate.Length} 완료");
        }

        private unsafe void ReallocProgress()
        {
            _progressCounter = 0;
            Marshal.FreeHGlobal(_pProgress);
            _pProgress = Marshal.AllocHGlobal(sizeof(float));
            *((float*)_pProgress.ToPointer()) = 0.0f;
        }

        private void OnDestroy()
        {
            Marshal.FreeHGlobal(_pProgress);
        }
    }
}