using System.Collections.Generic;
using UnityEngine;

namespace HTH.FSM
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : FSM 베이스 클래스.
    /// </summary>
    public abstract class StateMachine
    {
        public int currentID; // state id
        public IState current;
        protected Dictionary<int, IState> states;
        protected GameObject owner;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public StateMachine(GameObject owner)
        {
            this.owner = owner;
            states = new Dictionary<int, IState>();

            InitStates();

            current = states[currentID];
        }

        public abstract void InitStates();

        public void Update()
        {
            current.Update();
        }

        public bool ChangeState(int nextID)
        {
            if (currentID == nextID)
                return false;

            if (states[nextID].canExecute)
            {
                current.Stop();
                current = states[nextID];
                currentID = nextID;
                return true;
            }

            return false;
        }
    }
}