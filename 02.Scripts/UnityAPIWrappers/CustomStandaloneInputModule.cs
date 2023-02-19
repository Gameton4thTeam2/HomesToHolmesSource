using UnityEngine;
using UnityEngine.EventSystems;

namespace HTH.UnityAPIWrappers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_26
    /// 설명    : 마우스포인터의 데이터/ 포인팅된 오브젝트의 종류를 판별하기위한 기능이 추가됨.
    /// </summary>
    public class CustomStandaloneInputModule : StandaloneInputModule
    {
        public bool TryGetPointerData(out PointerEventData pointerData, int pointerId = kMouseLeftId)
        {
            return m_PointerData.TryGetValue(pointerId, out pointerData);
        }

        /// <summary>
        /// RayCaster 종류에 따른 GameObject 포인터 캐스팅 발생 여부 체크 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ignoreMask"> 캐스팅에서 무시할 레이어 마스크 </param>
        /// <param name="pointerId"> 포인터종류 </param>
        /// <param name="isCovariant"> 공변성적용 여부 </param>
        /// <returns></returns>
        public bool IsPointerOverGameObject<T>(LayerMask ignoreMask, int pointerId = kMouseLeftId, bool isCovariant = false)
            where T : BaseRaycaster
        {
            if (IsPointerOverGameObject(pointerId))
            {
                PointerEventData pointerEventData;
                if (m_PointerData.TryGetValue(pointerId, out pointerEventData))
                {
                    if ((1 << pointerEventData.pointerCurrentRaycast.gameObject.layer & ignoreMask) == 0)
                    {
                        BaseRaycaster module = pointerEventData.pointerCurrentRaycast.module;
                        return isCovariant ? module is T : module.GetType() == typeof(T);
                    }
                }
            }
            return false;
        }
    }
}