using Cysharp.Threading.Tasks;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 실패 실행
    /// </summary>
    public class Failure : Behaviour
    {
        public override async UniTask<Result> Invoke()
        {
            return Result.Failure;
        }
    }
}
