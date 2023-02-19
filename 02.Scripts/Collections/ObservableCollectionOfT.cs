using System;
using System.Collections.ObjectModel;

namespace HTH.Collections
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 변경을 통지하는 Generic Collection 
    /// </summary>
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged<T>
    {
        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;
        public event Action<T> ItemChanged;
        public event Action CollectionChanged;

        public T Find(Predicate<T> match)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (match.Invoke(Items[i]))
                    return Items[i];
            }
            return default(T);
        }

        public int FindIndex(Predicate<T> match)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (match.Invoke(Items[i]))
                    return i;
            }
            return -1;
        }

        public void Refresh()
        {
            CollectionChanged?.Invoke();
        }

        public void Set(int index, T item)
        {
            SetItem(index, item);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            ItemAdded?.Invoke(item);
            CollectionChanged?.Invoke();
        }

        protected override void RemoveItem(int index)
        {
            T removed = Items[index];
            base.RemoveItem(index);
            ItemRemoved?.Invoke(removed);
            CollectionChanged?.Invoke();
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            ItemChanged?.Invoke(item);
            CollectionChanged?.Invoke();
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            CollectionChanged?.Invoke();
        }
    }
}