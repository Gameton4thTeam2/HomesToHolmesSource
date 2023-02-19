using UnityEngine;

namespace HTH.UnityAPIWrappers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 애니메이터 래퍼
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorWrapper : MonoBehaviour
    {
        public bool isPreviousStateFinished { get; }
        public bool isPreviousMachineFinished { get; }
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
    }
}