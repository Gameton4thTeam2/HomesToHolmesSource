using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace HTH.InputHandlers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_11
    /// 설명    : 박스콜라이더들의 Transform 이 변경되었을때 충돌하게될 다른 콜라이더들을 미리 시뮬레이션하고 결과통지
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BoxTriggersCaster : MonoBehaviour
    {
        public static BoxTriggersCaster instance;
        public bool isCasted;
        public bool isBusy;
        public int triggerNum; // BoxCollider 풀링 갯수
        private BoxCollider[] _triggers; // 시뮬레이션용 콜라이더
        private LayerMask _targetMask;
        private event Action<bool> _onCasted;
        private Vector3 _offset = Vector3.up * 0.001f;
        private float _toleranceRate = 0.05f;
        public CancellationTokenSource cts = new CancellationTokenSource();


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        /// <summary>
        /// 박스콜라이더들이 어떤 위치,각도로 변경되었을 경우 충돌하게 될 다른 콜라이더가 있는지 확인하는 함수
        /// </summary>
        /// <param name="original"> 시뮬레이션 해보고싶은 게임오브젝트의 박스콜라이더들 </param>
        /// <param name="position"> 변경하려는 위치 </param>
        /// <param name="rotation"> 변경하려는 회전</param>
        /// <param name="targetMask"> 감지하려는 레이어마스크 </param>
        /// <returns></returns>
        public BoxTriggersCaster CastAsync(BoxCollider[] original, Vector3 position, Quaternion rotation, LayerMask targetMask)
        {
            if (isBusy)
                return this;

            if (original == null)
                return this;

            isBusy = true;

            UniTask.Create(async () =>
            {
                for (int i = 0; i < triggerNum; i++)
                {
                    if (i < original.Length)
                    {
                        _triggers[i].center = original[i].center + _offset;
                        _triggers[i].size = original[i].size * (1.0f - _toleranceRate);
                        _triggers[i].enabled = true;
                    }
                    else
                    {
                        _triggers[i].center = Vector3.up * 5000.0f;
                        _triggers[i].size = Vector3.zero;
                        _triggers[i].enabled = false;
                    }
                }
                transform.position = position;
                transform.rotation = rotation;
                _targetMask = targetMask;
                gameObject.SetActive(true);
                await UniTask.WaitForFixedUpdate(cts.Token);
                _onCasted?.Invoke(isCasted);
                isCasted = false;
            });
            return this;
        }

        public BoxTriggersCaster OnCasted(Action<bool> onCasted)
        {
            _onCasted = onCasted;
            _onCasted += (result) => gameObject.SetActive(false);
            isBusy = false;
            return this;
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            instance = this;
            _triggers = new BoxCollider[triggerNum];
            for (int i = 0; i < triggerNum; i++)
            {
                _triggers[i] = gameObject.AddComponent<BoxCollider>();
                _triggers[i].isTrigger = true;
                _triggers[i].enabled = false;
            }
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        private void OnTriggerStay(Collider other)
        {
            isCasted = (1 << other.gameObject.layer & _targetMask) > 0;

            if (isCasted)
                Debug.Log($"[BoxTriggersCaster] : Casted {other.gameObject}");

            _onCasted?.Invoke(isCasted);
        }
    }
}
