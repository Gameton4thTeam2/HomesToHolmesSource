using Cysharp.Threading.Tasks;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 행동트리 노드 베이스
    /// </summary>
    public abstract class Behaviour
    {
        public enum Result
        {
            Success,
            Failure,
            Running
        }

        public abstract UniTask<Result> Invoke();
    }
}
