using System;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.FSM
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : FSM 의 상태 베이스
    /// </summary>
    public abstract class State : IState
    {
        protected GameObject owner;
        protected Func<bool> condition;
        public int id { get; set; }
        public int current { get; set; }
        public virtual bool canExecute => condition.Invoke();
        public List<KeyValuePair<Func<bool>, int>> transitions { get; set; }


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public State(int id, GameObject owner, Func<bool> executeCondition)
        {
            this.id = id;
            this.owner = owner;
            this.condition = executeCondition;
        }

        public virtual void Execute()
        {
            current = 0;
        }

        public void Stop()
        {
        }

        public void MoveNext()
        {
            current++;
        }

        public int Update()
        {
            int nextId = id;

            foreach (var transition in transitions)
            {
                if (transition.Key.Invoke())
                {
                    nextId = transition.Value;
                    break;
                }
            }

            return nextId;
        }
    }
}
