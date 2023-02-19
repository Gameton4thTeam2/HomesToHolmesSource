using Cysharp.Threading.Tasks;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 행동트리 루트
    public class Root : Behaviour, IChild
    {
        public Behaviour child { get; set; }

        public override async UniTask<Result> Invoke()
        {
            return await child.Invoke();
        }
    }
}
