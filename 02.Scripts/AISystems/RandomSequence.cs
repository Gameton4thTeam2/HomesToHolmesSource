using Cysharp.Threading.Tasks;
using System;
using System.Linq;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 랜덤하게 모든 자식 순회
    public class RandomSequence : Composite
    {
        public override async UniTask<Result> Invoke()
        {
            Result result;
            foreach (var child in children.OrderBy(c => Guid.NewGuid()))
            {
                result = await child.Invoke();
                if (result == Result.Failure)
                    return result;
            }

            return Result.Success;
        }
    }
}
