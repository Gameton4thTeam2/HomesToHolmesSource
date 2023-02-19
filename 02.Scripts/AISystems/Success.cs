using Cysharp.Threading.Tasks;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 성공 반환
    public class Success : Behaviour
    {
        public override async UniTask<Result> Invoke()
        {
            return Result.Success;
        }
    }
}
