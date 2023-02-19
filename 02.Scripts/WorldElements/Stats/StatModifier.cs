using System;

/// <summary>
/// 작성자  : 조영민
/// 작성일  : 2023_01_09
/// 설명    : 스탯 조정자. 스탯에 추가해서 스탯을 특정 수치 조정할 수 있음.
/// </summary>
namespace HTH.WorldElements
{
    public enum StatModType
    {
        Flat,
        PercentAdd,
        PercentMul,
    }

    [Serializable]
    public class StatModifier
    {
        public readonly int id;
        public readonly int value;
        public readonly StatModType modType;


        public StatModifier(int id, int value, StatModType modType)
        {
            this.id = id;
            this.value = value;
            this.modType = modType;
        }
    }
}