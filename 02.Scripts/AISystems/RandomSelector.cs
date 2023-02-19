using Cysharp.Threading.Tasks;
using System;
using System.Linq;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 랜덤하게 실행한 자식들중 성공 선택
    public class RandomSelector : Composite
    {
        public override async UniTask<Result> Invoke()
        {
            Result result;
            foreach (var child in children.OrderBy(c => Guid.NewGuid()))
            {
                result = await child.Invoke();
                if (result == Result.Success)
                    return result;
            }

            return Result.Failure;
        }
    }
}
