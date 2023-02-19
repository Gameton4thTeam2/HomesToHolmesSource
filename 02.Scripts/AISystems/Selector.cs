using Cysharp.Threading.Tasks;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 성공 선택
    public class Selector : Composite
    {
        public override async UniTask<Result> Invoke()
        {
            Result result;
            foreach (var child in children)
            {
                result = await child.Invoke();
                if (result == Result.Success)
                    return result;
            }

            return Result.Failure;
        }
    }
}
