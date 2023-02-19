using System;

namespace HTH.Collections
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : Generic Collection 변경 통지
    /// </summary>
    public interface INotifyCollectionChanged<T>
    {
        event Action<T> ItemAdded;
        event Action<T> ItemRemoved;
        event Action<T> ItemChanged;
        event Action CollectionChanged;
    }
}
