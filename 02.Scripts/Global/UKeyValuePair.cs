using System;

namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : UnityEditor 인스펙터용 KeyValuePair
    /// </summary>
    [Serializable]
    public struct UKeyValuePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;

        public UKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
}