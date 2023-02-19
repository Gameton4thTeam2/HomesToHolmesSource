using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTH.IDs;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 월드상의 아이템. 방의 스탯을 조정하는 요소를 포함하고있음.
    /// </summary>
    public class Item : MonoBehaviour
    {
        public ItemID id;
        public List<StatModifier> statModifiers;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void PlayPointAnimation()
        {
            StartCoroutine(E_PointAnimation());
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void OnEnable()
        {
            StartCoroutine(E_PointAnimation());
        }               

        IEnumerator E_PointAnimation()
        {
            float timeMark = Time.time;
            while (Time.time - timeMark < 0.1f)
            {
                transform.localScale = (1.0f + 0.3f * Mathf.Sin((Time.time - timeMark) * 30)) * Vector3.one;
                yield return null;
            }
            transform.localScale = Vector3.one;
        }
    }
}