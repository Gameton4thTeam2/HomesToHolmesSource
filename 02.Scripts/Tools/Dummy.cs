using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_27
    /// 설명    : 코루틴 등을 처리하기위한 일회성 쓰레기컴포넌트
    /// </summary>
    public class Dummy : MonoBehaviour
    {
        public void InstantiateGameObjectInCoroutine<T>(T original, Action<T> onFinished) where T : MonoBehaviour
        {
            StartCoroutine((E_Instantiate<T>(original, onFinished)));
        }

        public void InstantiateGameObjectInCoroutine<T>(T original, Vector3 position, Action<T> onFinished) where T : MonoBehaviour
        {
            StartCoroutine((E_Instantiate<T>(original, position, onFinished)));
        }

        public IEnumerator E_Instantiate<T>(T original, Action<T> onFinished) where T : MonoBehaviour
        {
            onFinished?.Invoke(Instantiate(original));
            yield return null;
        }

        public IEnumerator E_Instantiate<T>(T original, Vector3 position, Action<T> onFinished) where T : MonoBehaviour
        {
            onFinished?.Invoke(Instantiate(original, position, Quaternion.identity));
            yield return null;
        }
    }
}
