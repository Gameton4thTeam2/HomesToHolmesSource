using System;
using System.Collections;
using System.Collections.Generic;
using HTH.Collections;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_18
    /// 설명    : Collection 을 Data 로가지는 모델 베이스클래스
    /// </summary>
    [Serializable]
    public abstract class CollectionDataModelBase<T, K> 
        : SingletonBase<T, K>, ICollection<T>, INotifyCollectionChanged<T>
        where T : IComparable<T>
        where K : SingletonBase<T, K>
    {
        public int Count => Items.Count;

        public bool IsReadOnly => false;

        public List<T> Items = new List<T>();
        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;
        public event Action<T> ItemChanged;
        public event Action CollectionChanged;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public virtual void Set(int index, T item)
        {
            Items[index] = item;
            Save();
            ItemChanged?.Invoke(item);
            CollectionChanged?.Invoke();
        }

        public virtual void Add(T item)
        {
            Items.Add(item);
            Save();
            ItemAdded?.Invoke(item);
            CollectionChanged?.Invoke();
        }

        public virtual void AddRange(IEnumerable<T> item)
        {
            Items.AddRange(item);
            Save();
            CollectionChanged?.Invoke();
        }

        public virtual bool Remove(T item)
        {
            bool removed = false;
            if (Items.Remove(item))
            {
                Save();
                ItemRemoved?.Invoke(item);
                CollectionChanged?.Invoke();
                removed = true;
            }
            return removed;
        }

        public virtual bool Remove(Predicate<T> match)
        {
            bool removed = false;
            int index = Items.FindIndex(match);
            if (index >= 0)
            {
                T item = Items[index];
                if (Items.Remove(Items[index]))
                {
                    Save();
                    ItemRemoved?.Invoke(item);
                    removed = true;
                }
            }
            return removed;
        }

        public virtual void RemoveAt(int index)
        {
            T item = Items[index];
            Items.RemoveAt(index);
            Save();
            ItemRemoved?.Invoke(item);
        }

        public virtual bool Change(T item, Predicate<T> match)
        {
            bool changed = false;
            int index = Items.FindIndex(match);
            if (index >= 0)
            {
                Items[index] = item;
                Save();
                ItemChanged?.Invoke(item);
                CollectionChanged?.Invoke();
                changed = true;
            }

            return changed;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public abstract void Load();

        public abstract void Save();


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }
    }
}