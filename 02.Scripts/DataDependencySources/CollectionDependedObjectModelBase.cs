using System;
using System.Collections.Generic;
using HTH.Collections;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_13
    /// 설명    : Collection data source를 참조하는 Presenter/ ViewModel base
    public abstract class CollectionDependedObjectModelBase<T, K>
        where T : ICollection<K>, INotifyCollectionChanged<K>
        where K : IComparable<K>
    {
        public Source source;
        public AddCommand addCommand;
        public RemoveCommand removeCommand;

        #region Source
        public class Source : ObservableCollection<K>
        {
            public Source(IEnumerable<K> copy)
            {
                foreach (var item in copy)
                {
                    Items.Add(item);
                }
            }
        }
        #endregion

        /// <summary>
        /// 자식 클래스 생성자에서 반드시 호출해주어야함.
        /// </summary>
        public virtual void InitializeSource(T data)
        {
            source = new Source(data);
            data.ItemAdded += (item) =>
            {
                source.Add(item);
            };
            data.ItemRemoved += (item) =>
            {
                source.Remove(item);
            };
            data.ItemChanged += (item) =>
            {
                source.Set(source.FindIndex(x => x.CompareTo(item) == 0), item);
            };
            data.CollectionChanged += () =>
            {
                source.Refresh();
            };

            addCommand = new AddCommand(data);
            removeCommand = new RemoveCommand(data);

            UnityEngine.Debug.Log($"DataModel {typeof(T).Name} 의 소스 초기화 완료. (데이터갯수 : {source.Count} ");
        }

        #region Add Command
        public class AddCommand
        {
            private T _data;
            public AddCommand(T data)
            {
                _data = data;
            }

            public virtual bool CanExecute(K item)
            {
                return true;
            }

            public virtual void Execute(K item)
            {
                _data.Add(item);
            }

            public virtual bool TryExecute(K item)
            {
                if (CanExecute(item))
                {
                    Execute(item);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Remove Command
        public class RemoveCommand
        {
            private T _data;
            public RemoveCommand(T data)
            {
                _data = data;
            }

            public virtual bool CanExecute(K item)
            {
                return _data.Contains(item);
            }

            public virtual void Execute(K item)
            {
                _data.Remove(item);
            }

            public virtual bool TryExecute(K item)
            {
                if (CanExecute(item))
                {
                    _data.Remove(item);
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}
