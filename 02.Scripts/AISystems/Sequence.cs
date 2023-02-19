using Cysharp.Threading.Tasks;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 실패없을때 까지 자식순회
    public class Sequence : Composite
    {
        public override async UniTask<Result> Invoke()
        {
            Result result;
            UnityEngine.Debug.Log("[BehaviourTree][Sequence] : Starated ...");
            foreach (var child in children)
            {
                result = await child.Invoke();
                UnityEngine.Debug.Log($"[BehaviourTree][Sequence] : Child Invoked {child}, result : {result}...");
                if (result == Result.Failure)
                    return result;
            }

            return Result.Success;
        }
    }
}
