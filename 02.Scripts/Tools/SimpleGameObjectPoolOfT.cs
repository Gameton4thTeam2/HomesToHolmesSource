using System.Collections.Generic;
using UnityEngine;

namespace HTH.Tools
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_13
    /// 설명    : GameObject 를 원하는 갯수만큼 소환할 수 있는 풀
    /// </summary>
    public class SimpleGameObjectPool<T> where T : MonoBehaviour
    {
        private T _prefab;
        private Transform _parent;
        private Queue<T> _queue;
        private List<T> _list;
        private List<T> _listSpawned;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public SimpleGameObjectPool(T prefab, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _queue = new Queue<T>();
            _list = new List<T>();
            _listSpawned = new List<T>();
        }

        /// <summary>
        /// 기존 오브젝트 모두 되돌리고 다시 원하는 갯수만큼 활성화. 
        /// 모자라면 Instantiate 해서라도 활성화 해줌.
        /// </summary>
        /// <param name="num"> 활성화 하고싶은 GameObject 갯수 </param>
        /// <returns> 활성화된 GameObject 들 </returns>
        public IEnumerable<T> Refresh(int num)
        {
            _queue.Clear();
            _listSpawned.Clear();
            foreach (T item in _list)
            {
                item.gameObject.SetActive(false);
                _queue.Enqueue(item);
            }

            T tmp;
            for (int i = 0; i < num; i++)
            {
                if (_queue.Count <= 0)
                {
                    tmp = GameObject.Instantiate(_prefab, _parent);
                    _list.Add(tmp);
                    _queue.Enqueue(tmp);
                }

                tmp = _queue.Dequeue();
                tmp.gameObject.SetActive(true);
                _listSpawned.Add(tmp);
            }

            return _listSpawned;
        }

        /// <summary>
        ///  원하는 갯수만큼 추가 소환. 모자라면 Instantiate 해서라도 소환해줌
        /// </summary>
        /// <param name="num"> 소환하고싶은 GameObject 갯수 </param>
        /// <returns> 추가로 소환된 GameObject 들 </returns>
        public IEnumerable<T> Spawn(int num)
        {
            T tmp;
            List<T> justSpawned = new List<T>();
            for (int i = 0; i < num; i++)
            {
                if (_queue.Count <= 0)
                {
                    tmp = GameObject.Instantiate(_prefab, _parent);
                    _list.Add(tmp);
                    _queue.Enqueue(tmp);
                }

                tmp = _queue.Dequeue();
                tmp.gameObject.SetActive(true);
                justSpawned.Add(tmp);
                _listSpawned.Add(tmp);
            }

            return justSpawned;
        }

        /// <summary>
        /// 모든 요소 풀로 되돌림
        /// </summary>
        public void ReturnAll()
        {
            _queue.Clear();
            foreach (T item in _list)
            {
                item.gameObject.SetActive(false);
                _queue.Enqueue(item);
            }
            _listSpawned.Clear();
        }

        /// <summary>
        /// 소환된 (활성화된) 요소 반환
        /// </summary>
        public IEnumerable<T> GetSpawnedObjects() => _listSpawned;

        public IEnumerable<T> GetAllObjects() => _list;
    }
}