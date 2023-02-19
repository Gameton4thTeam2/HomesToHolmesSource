using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : UIMonoBehaviour 관리자. 
    ///          로드된 UIMonoBehavior 는 이 매니저에 등록되고 , 
    ///          UIMonoBehavior.Show() 호출시 해당 UIMonoBehavior 가 매니저의 스택에 쌓임. 
    ///          UIMonoBehavior.Hide() 호출시 해당 UIMonoBehavior 가 매니저이 스택에서 빠짐. 
    ///          Show() 가 호출된 순서대로 Hide() 가 호출되지 않을경우 에러.
    /// </summary>
    public class UIManager : SingletonBase<UIManager>
    {
        public List<IUI> uis = new List<IUI>();
        public Stack<IUI> uisShown = new Stack<IUI>();


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Register(IUI ui)
        {
            uis.Add(ui);
        }

        public void Remove(IUI ui)
        {
            uis.Remove(ui);
        }

        public bool Push(IUI ui)
        {
            if (uisShown.Count > 0 &&
                uisShown.Peek() == ui)
            {
                Debug.LogWarning($"[UIManager] : {uisShown.Peek()} is already pushed.");
                return false;
            }

            uisShown.Push(ui);
            ui.sortingOrder = uisShown.Count;
            Debug.Log($"{ui} is Pushed");
            return true;
        }

        public bool Pop(IUI ui)
        {            
            if (ui == uisShown.Peek())
            {
                uisShown.Pop();

                if (uisShown.Count > 0)
                    uisShown.Peek().sortingOrder = uisShown.Count;

                Debug.Log($"{ui} is pop");
                return true;
            }
            else
            {
                Debug.LogWarning($"[UIManager] : Cannot hide {ui}. Try to hide latest ui ({uisShown.Peek()}) first.");
                return false;
            }
        }

        public void HideLast()
        {
            if (uisShown.Count > 0)
            {
                uisShown.Peek().Hide();                
            }
            else
            {
                Debug.LogWarning("[UIManager] : Cannot hide last. Stack is empty");
            }
        }

        public void HideAll()
        {
            foreach (var ui in uis)
                ui.HideUnmanaged();
            uisShown.Clear();
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override protected void Init()
        {
            base.Init();
            HideAll();
        }
    }
}