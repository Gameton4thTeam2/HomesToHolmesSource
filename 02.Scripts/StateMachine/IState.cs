using System;
using System.Collections.Generic;

namespace HTH.FSM
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : FSM 상태 인터페이스
    /// </summary>
    public interface IState
    {
        int id { get; set; }
        int current { get; set; }
        bool canExecute { get; }
        List<KeyValuePair<Func<bool>, int>> transitions { get; set; }
        void Execute();
        void Stop();
        int Update();
        void MoveNext();
    }
}