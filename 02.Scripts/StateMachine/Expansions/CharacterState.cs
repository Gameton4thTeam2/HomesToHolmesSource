using System;
using System.Collections.Generic;
using UnityEngine;
using HTH.UnityAPIWrappers;

namespace HTH.FSM
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 캐릭터용 FSM 상태
    /// </summary>
    public abstract class CharacterState : State
    {
        protected AnimatorWrapper animator;
        protected CharacterState(int id, GameObject owner, Func<bool> executeCondition) : base(id, owner, executeCondition)
        {
            animator = owner.GetComponent<AnimatorWrapper>();
        }

        public override bool canExecute => base.canExecute &&
                                           animator.isPreviousStateFinished &&
                                           animator.isPreviousMachineFinished;
    }
}
