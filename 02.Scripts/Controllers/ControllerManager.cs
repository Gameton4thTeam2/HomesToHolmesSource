using System.Collections.Generic;
using UnityEngine;

namespace HTH.InputHandlers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_26
    /// 설명    : IController 를 관리하는 클래스. 한번에 하나의 컨트롤러만 사용가능하도록함
    /// </summary>
    public class ControllerManager : SingletonBase<ControllerManager>
    {
        public List<IController> controllers;
        public IController current;
        private IController _default;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Register(IController controller)
        {
            if (controllers.Contains(controller) == false)
                controllers.Add(controller);
        }

        public void RegisterAsDefault(IController defaultController)
        {
            Register(defaultController);
            _default = defaultController;
            HandOverControlTo(_default);
        }

        /// <summary>
        /// 제어권을 다른 컨트롤러에게 양도
        /// </summary>
        public bool HandOverControlTo(IController requester)
        {
            bool isSuccess = false;
            foreach (var controller in controllers)
            {
                if (controller == requester)
                {
                    controller.controllable = true;
                    current = controller;
                    Debug.Log($"[ControllerManager] : {requester} 가 제어권 획득. ");
                    isSuccess = true;
                }
                else
                {
                    controller.controllable = false;
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// 제어권을 다른 컨트롤러에게 양도
        /// </summary>
        public bool HandOverControlFromTo(IController requester, IController other)
        {
            if (current == requester)
            {
                requester.controllable = false;
                other.controllable = true;
                current = other;
                
                Debug.Log($"[ControllerManager] : {other} 가 제어권 획득. ");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 제어권 반환
        /// </summary>
        public bool ReturnControl(IController returner)
        {
            if (current != returner)
            {
                Debug.LogWarning($"[ControllerManager] : {returner} 가 제어권을 반환하려고 시도했지만 원래 제어권이 없었습니다. ");
                return false;
            }
            
            if (_default == returner)
            {
                Debug.LogError($"[ControllerManager] : {returner} 가 제어권을 반환하려고 시도했지만 기본값이므로 제어권 반환은 불가능하고, 양도만 가능합니다.");
                return false;
            }

            returner.controllable = false;
            current = _default;
            current.controllable = true;
            Debug.Log($"[ControllerManager] : {_default} 가 제어권 획득. ");
            return true;
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            controllers = new List<IController>();
            base.Init();
        }
    }
}
