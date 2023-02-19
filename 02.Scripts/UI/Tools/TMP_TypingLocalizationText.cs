using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text;
using System;
using HTH.GameSystems;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_11
    /// 설명    : 타이핑효과 적용시킨 TMP_Text
    /// text 에 직접 쓰지않고 SetText() 로 text 를 갱신해야 타이핑 이펙트가 실행됨.
    /// </summary>
    public class TMP_TypingLocalizationText : TextMeshProUGUI, IPointerClickHandler
    {
        new public string text
        {
            get => base.text;
            set
            {
                if (_isTyping)
                    ShowOriginText();

                if (string.IsNullOrEmpty(value))
                    return;

                string tableName = $"{value.Split('_')[0]}_{value.Split('_')[1]}";
                _originText = Localization.instance[tableName, value];
                if (_originText.Contains("#Username"))
                {
                    _originText = _originText.Replace("#Username", User.nickName);
                }    

                _buffer.Clear();
                base.text = String.Empty;

                StartTyping();
            }
        }
        public event Action OnTypingStarted;
        public event Action OnTypingFinished;
        [SerializeField] private float _typingDelay = 0.02f;
        private string _originText = String.Empty;
        private StringBuilder _buffer = new StringBuilder(); // 타이핑 내용 임시 작성 버퍼
        private Coroutine coroutine = null;
        private bool _isTyping;


        //===============================================================================================
        //                                  Public Methods
        //===============================================================================================

        public void StartTyping()
        {
            if (gameObject.activeSelf == false)
                return;

            coroutine = StartCoroutine(E_TypingEffectCoroutine());
            _isTyping = true;
            OnTypingStarted?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ShowOriginText();
        }


        //===============================================================================================
        //                                  Private Methods
        //===============================================================================================

        protected override void OnEnable()
        {
            base.OnEnable();
            if(text != null)
                text = base.text;
        }

        IEnumerator E_TypingEffectCoroutine()
        {
            float timer = _typingDelay;

            for (int i = 0; i < _originText.Length; i++)
            {
                _buffer.Append(_originText[i]);
                base.text = _buffer.ToString();
                //while (timer > 0)
                //{
                //    timer -= 0.0333f;
                //}
                //timer = _typingDelay;
                yield return null;

            }
            base.text = _originText;
            _buffer.Clear();
            coroutine = null;
            _isTyping = false;
            OnTypingFinished?.Invoke();
        }

        /// <summary>
        /// 타이핑끝내고 원래 텍스트 보여줌
        /// </summary>
        public void ShowOriginText()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            base.text = _originText;
            _buffer.Clear();
            _isTyping = false;

            OnTypingFinished?.Invoke();
        }
    }
}
