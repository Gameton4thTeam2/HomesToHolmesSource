using System;
using System.Reflection;

namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_23
    /// 설명    : 제네릭 싱글톤 베이스. 
    /// TWrapper 에 Activator 로 생성할 인스턴스의 타입을 넣어줌.
    /// </summary>
    public abstract class SingletonBase<TData, TWrapper>
        where TWrapper : SingletonBase<TData, TWrapper>
    {
        private static volatile TWrapper _instance;
        private static object _spinLock = new object();

        public static TWrapper instance
        {
            get
            {
                lock (_spinLock)
                {
                    if (_instance == null)
                    {
                        try
                        {
                            _instance = Activator.CreateInstance(typeof(TWrapper)) as TWrapper;
                            _instance.Init();
                        }
                        catch
                        {
                            _instance = Activator.CreateInstance(typeof(TWrapper), false) as TWrapper;
                            _instance.Init();
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Init() 
        {
            UnityEngine.Debug.Log($"DataModel {typeof(TWrapper).Name} 의 인스턴스 생성 완료. ");
        }
    }

    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_23
    /// 설명    : 제네릭 싱글톤 베이스.
    /// </summary>
    public abstract class SingletonBase<T>
        where T : SingletonBase<T>
    {
        private static volatile T _instance;
        private static object _spinLock = new object();
    
        public static T instance
        {
            get
            {
                lock (_spinLock)
                {
                    if (_instance == null)
                    {
                        try
                        {
                            _instance = Activator.CreateInstance(typeof(T)) as T;
                            //ConstructorInfo constructorInfo = typeof(T).GetConstructor(new Type[] { });
                            //_instance = constructorInfo.Invoke(new object[] { }) as T;
                            _instance.Init();
                        }
                        catch
                        {
                            _instance = Activator.CreateInstance(typeof(T), false) as T;
                            _instance.Init();
                        }
                    }
                }
                return _instance;
            }
        }
    
        protected virtual void Init()
        {
            UnityEngine.Debug.Log($"DataModel {typeof(T).Name} 의 인스턴스 생성 완료. ");
        }
    }
}