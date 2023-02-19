
using Cysharp.Threading.Tasks;
using System;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 단말 행동
    /// </summary>
    public class Execution : Behaviour
    {
        private Func<Result> _execute;


        public Execution(Func<Result> execute)
        {
            _execute = execute;
        }

        public override async UniTask<Result> Invoke()
        {
            Result result;
            while (true)
            {
                result = _execute.Invoke();

                if (result == Result.Running)
                    await UniTask.Yield();
                else
                    break;
            }

            return result;
        }
    }
}
