using System;
using Cysharp.Threading.Tasks;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 조건 행동
    /// </summary>
    public class Condition : Behaviour, IChild
    {
        public Behaviour child { get; set; }
        private Func<bool> _condition;


        public Condition(Func<bool> condition)
        {
            _condition = condition;
        }

        public override async UniTask<Result> Invoke()
        {
            if (_condition.Invoke())
                return await child.Invoke();

            return Result.Failure;
        }
    }
}
