using UnityEngine;
using UnityEngine.AddressableAssets;
namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_23
    /// 설명    : MonoBehavior 용 제네릭 싱글톤 베이스.
    /// Resources 에서 로드하거나 Awake 에서 초기화함. 
    /// </summary>
    public abstract class SingletonMonoBase<T> : MonoBehaviour
        where T : SingletonMonoBase<T>
    {
        private static volatile T _instance;
        private static volatile bool _lockAwake;
        private static object _spinLock = new object();
        private static volatile bool _notExist;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    // 에셋을 로드할 수 없음
                    if (_notExist)
                    {
                        return null;
                    }

                    // 에셋 로드시도
                    T asset = Resources.Load<T>(typeof(T).Name);                    
                    if (asset == null)
                    {
                        _notExist = true;
                        return null;
                    }

                    // 에셋 로드 성공
                    lock (_spinLock)
                    {   
                        if (_lockAwake == false)
                        {
                            _lockAwake = true;
                            try
                            {
                                _instance = Instantiate(asset);
                                _instance.Init();
                            }
                            catch
                            {
                                Debug.LogError($"[{typeof(T)}] : Failed to find this asset in Resources directory.");
                            }
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void Init() { }

        protected virtual void Awake()
        {
            lock (_spinLock)
            {
                if (_lockAwake)
                {
                    _lockAwake = false;
                    return;
                }
                else
                {
                    _instance = (T)this;
                    _instance.Init();
                }
            }
        }
    }
}