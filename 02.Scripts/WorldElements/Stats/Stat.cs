using System;
using System.Collections.Generic;
using UnityEngine;
using HTH.IDs;

/// <summary>
/// 작성자  : 조영민
/// 작성일  : 2023_01_09
/// 설명    : 스탯 ( 의뢰 및 유저의 방 점수를 평가하기위한 척도 ). StatModifier 를 추가해서 조정할 수 있음. 
/// </summary>
namespace HTH.WorldElements
{
    [Serializable]
    public class Stat
    {
        public StatID id;
        public int value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value <= min)
                {
                    value = min;
                    OnValueMin?.Invoke();
                }
                else if (value >= max)
                {
                    value = max;
                    OnValueMax?.Invoke();
                }

                _value = value;
                OnValueChanged?.Invoke(value);
                valueModified = CalcValueModified();
            }
        }
        private int _value;
        public int min = 0;
        public int max = 999999;        
        public int valueModified
        {
            get
            {
                return _valueModified;
            }
            set
            {
                _valueModified = value;
                OnValueModified?.Invoke(value);
            }
        }
        private int _valueModified;
        public List<StatModifier> modifiers = new List<StatModifier>();
        public Action<int> OnValueChanged;
        public Action OnValueMin;
        public Action OnValueMax;
        public Action<int> OnValueModified;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
            valueModified = CalcValueModified();
        }

        public void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier);
            valueModified = CalcValueModified();
        }

        public int CalcValueModified()
        {
            int sumFlat = 0;
            float sumPercentAdd = 0.0f;
            float mulPercentMul = 1.0f;
            StatModifier mod;

            for (int i = 0; i < modifiers.Count; i++)
            {
                mod = modifiers[i];

                switch (mod.modType)
                {
                    case StatModType.Flat:
                        {
                            sumFlat += mod.value;
                        }
                        break;
                    case StatModType.PercentAdd:
                        {
                            sumPercentAdd += (mod.value / 100.0f);
                        }
                        break;
                    case StatModType.PercentMul:
                        {
                            mulPercentMul *= (mod.value / 100.0f);
                        }
                        break;
                    default:
                        break;
                }
            }

            return Mathf.RoundToInt((value + sumFlat) * sumPercentAdd * mulPercentMul);
        }
    }
}
