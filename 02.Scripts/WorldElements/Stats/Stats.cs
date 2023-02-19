using System;
using System.Collections.Generic;

/// <summary>
/// 작성자  : 조영민
/// 작성일  : 2023_01_09
/// 설명    : 스탯 목록
/// </summary>
namespace HTH.WorldElements
{
    [Serializable]
    public class Stats
    {
        public List<Stat> list = new List<Stat>();
        public Stat this[int id] => list[id];
    }
}