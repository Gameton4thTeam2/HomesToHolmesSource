using Cysharp.Threading.Tasks;
using System.Diagnostics;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 모든자식 호출 후 정책에 따라 결과 반환
    /// </summary>
    internal class Parallel : Composite
    {
        private int _successPolicy;


        public Parallel(int successPolicy)
        {
            _successPolicy = successPolicy;
        }

        public override async UniTask<Result> Invoke()
        {
            Result result;
            int successCount = 0;
            UnityEngine.Debug.Log($"[BehaviourTree][Pararell] : start  {children.Count}");

            foreach (var child in children)
            {
                result = await child.Invoke();
                if (result == Result.Success)
                {
                    UnityEngine.Debug.Log($"[BehaviourTree][Pararell] : success child ");
                    successCount++;
                }
            }

            UnityEngine.Debug.Log($"[BehaviourTree][Pararell] : success policy : {successCount >= _successPolicy}");

            if (successCount >= _successPolicy)
                return Result.Success;
            else
                return Result.Failure;
        }
    }
}
