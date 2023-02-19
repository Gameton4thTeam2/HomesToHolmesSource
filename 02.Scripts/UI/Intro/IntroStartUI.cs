using HTH.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTH.GameSystems;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 권병석
    /// 작성일  : 2023_02_01
    /// 설명    : 인트로 시작 부분을 담당하는 UI
    /// </summary>
    public class IntroStartUI : UIMonoBehaviour<IntroStartUI>
    {
        public event Action OnChatingStart;
        public event Action OnChatingFinish;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_TypingLocalizationText _chatContent;
        [SerializeField] private Button _chatNext;
        private IEnumerator<ChatData> _chatEnumerator;
        private Coroutine _effectCoroutine;
        private bool _isEffecting;
        private bool _moveNextDisabled;

        public void Show(ChatsInfo chatsInfo)
        {
            _chatEnumerator = chatsInfo.chats.GetEnumerator();
            gameObject.SetActive(true);
            base.Show();
            StartCoroutine(E_FadeInCanvasGroup(5f,
                                               () =>
                                               {
                                                   OnChatingStart?.Invoke();
                                                   _canvasGroup.interactable = false;
                                               },
                                               () =>
                                               {
                                                   _canvasGroup.interactable = true;
                                                   _canvasGroup.blocksRaycasts = true;
                                                   MoveNextChatForce();
                                               })
            );
        }
    

        public void Show(ChatsInfo chatsInfo, Action onChatingStart = null, Action onChatingFinish = null)
        {
            OnChatingStart = onChatingStart;
            OnChatingFinish = onChatingFinish;
            Show(chatsInfo);
        }

        public void FinishChats()
        {
            _chatEnumerator.Reset();
            _chatEnumerator.Dispose();

            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);

            OnChatingFinish?.Invoke();
            gameObject.SetActive(false);
            base.Hide();
        }

        protected override void Init()
        {
            _chatNext.onClick.AddListener(MoveNextChat);
            _chatContent.OnTypingStarted += () => _moveNextDisabled = true;
            _chatContent.OnTypingFinished += () => _moveNextDisabled = false;
            base.Init();
        }

        private void MoveNextChat()
        {
            if (_moveNextDisabled)
                return;

            if (_isEffecting)
                return;

            if (gameObject.activeSelf == false)
                return;

            if (_chatEnumerator.MoveNext())
            {
                Debug.Log($"[ChattingUI] : Move next chat - {_chatEnumerator.Current.content}");
                ChatData chat = _chatEnumerator.Current;
                
                if(chat.content.Contains("[Name]"))
                {
                    chat.content = chat.content.Replace("[Name]", User.nickName);
                }
                _chatContent.text = chat.content;
                _moveNextDisabled = true;
            }
            else
            {
                FinishChats();
            }
        }

        private IEnumerator E_FadeInCanvasGroup(float duration, Action OnFadeStart, Action OnFadeFinish)
        {
            OnFadeStart?.Invoke();
            float timeMark = Time.time;
            float speed = 1.0f / duration;
            _canvasGroup.alpha = 0.0f;
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += (Time.time - timeMark) * speed;
                Debug.Log($"fading in ... {_canvasGroup.alpha}");
                yield return null;
            }
            _effectCoroutine = null;
            _isEffecting = false;
            OnFadeFinish?.Invoke();
        }

        private void MoveNextChatForce()
        {
            if (_chatEnumerator.MoveNext())
            {
                Debug.Log($"[ChattingUI] : Move next chat - {_chatEnumerator.Current.content}");
                ChatData chat = _chatEnumerator.Current;
                _chatContent.text = chat.content;
                _moveNextDisabled = true;
            }
            else
            {
                FinishChats();
            }
        }
    }
}