using HTH.DataModels;
using HTH.GameSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace HTH.UI
{
    /// <summary>
    /// 작성자      : 조영민
    /// 작성일      : 2023_01_12
    /// 설명        : 유저가 채팅을 넘기면서 순회할 수 있도록 하는 UI
    ///               채팅내용은 TMP_TypingText 를 사용하고있으며, 타이핑 이펙트가 진행중일때는 유저가 다음 챗으로 넘길 수 없음.
    ///               챗에 이펙트가 적용되어야 하는 경우, 이펙트 적용 중에도 유저가 다음 챗으로 넘길 수 없음.
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_12
    /// 설명        : 채팅 타입별 해당하는 UI 패널 ON/OFF 적용, NPC가 null 값일 때 처리하는 부분 추가 [2023_02_01, 권병석]
    ///               Localization 적용
    /// </summary>
    public class ChattingUI : UIMonoBehaviour<ChattingUI>
    {
        public event Action OnChatingStart;
        public event Action OnChatingFinish;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _chatIllust;
        [SerializeField] private RectTransform _chatIllustLeftPoint;
        [SerializeField] private RectTransform _chatIllustCenterPoint;
        [SerializeField] private RectTransform _chatIllustRightPoint;
        [SerializeField] private TMP_Text _chaterName;
        [SerializeField] private TMP_TypingLocalizationText _chatContent;
        [SerializeField] private Button _chatNext;
        [SerializeField] private GameObject _chattingBG;
        [SerializeField] private GameObject _ThinkingBG;
        [SerializeField] private GameObject _chaterNamePanel;
        private IEnumerator<ChatData> _chatEnumerator;
        private Coroutine _effectCoroutine;
        private bool _isEffecting;
        private bool _moveNextDisabled;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        /// <summary>
        /// 채팅 시작
        /// </summary>
        /// <param name="chatsInfo"> 시작하려는 채팅 정보.</param>
        public void Show(ChatsInfo chatsInfo)
        {
            _chatEnumerator = chatsInfo.chats.GetEnumerator();
            gameObject.SetActive(true);
            base.Show();
            StartCoroutine(E_FadeInCanvasGroup(1.0f,
                                               () =>
                                               {
                                                   OnChatingStart?.Invoke();
                                                   _canvasGroup.interactable = false;
                                               },
                                               () => 
                                               {
                                                   _canvasGroup.interactable = true;
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

        /// <summary>
        /// 채팅 끝. 
        /// </summary>
        public void FinishChats()
        {
            _chatEnumerator.Reset();
            _chatEnumerator.Dispose();

            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);

            OnChatingFinish?.Invoke();
            gameObject.SetActive(false);
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override protected void Init()
        {
            _chatNext.onClick.AddListener(MoveNextChat);
            _chatContent.OnTypingStarted += () => _moveNextDisabled = true;
            _chatContent.OnTypingFinished += () => _moveNextDisabled = false;
            base.HideUnmanaged();
        }
                
        /// <summary>
        /// 다음 챗으로 넘어감. 
        /// ChatData 에 따라 챗내용, 일러스트, 이펙트 등 설정
        /// </summary>
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
                _chatContent.text = chat.content;
                SetChattingType(chat.chattingType);
                if (chat.npcID != null)
                {
                    if (chat.illust != IllustType.Silhouette)
                        _chaterName.text = NPCAssets.instance[chat.npcID.value].name;
                    else
                        _chaterName.text = "???";
                    _chatIllust.sprite = NPCIllustAssets.instance[chat.npcID][chat.illust];
                    SetIllustPos(chat.illustPos);
                    DoChatEffect(chat.chatEffect.effectType, chat.chatEffect.duration);
                }
                _moveNextDisabled = true;
            }
            else
            {
                FinishChats();
            }            
        }        

        private void MoveNextChatForce()
        {
            if (_chatEnumerator.MoveNext())
            {
                Debug.Log($"[ChattingUI] : Move next chat - {_chatEnumerator.Current.content}");
                ChatData chat = _chatEnumerator.Current;
                _chatContent.text = chat.content;
                SetChattingType(chat.chattingType);
                if (chat.npcID != null)
                {
                    if (chat.illust != IllustType.Silhouette)
                        _chaterName.text = NPCAssets.instance[chat.npcID.value].name;
                    else
                        _chaterName.text = "???";
                    _chatIllust.sprite = NPCIllustAssets.instance[chat.npcID][chat.illust];
                    SetIllustPos(chat.illustPos);
                    DoChatEffect(chat.chatEffect.effectType, chat.chatEffect.duration);
                }
                _moveNextDisabled = true;
            }
            else
            {
                FinishChats();
            }
        }

        private void SetIllustPos(Pos pos)
        {
            switch (pos)
            {
                case Pos.Right:
                    {
                        _chatIllust.rectTransform.position = _chatIllustRightPoint.position;
                        _chatIllust.rectTransform.eulerAngles = Vector3.zero;
                    }
                    break;
                case Pos.Left:
                    {
                        _chatIllust.rectTransform.position = _chatIllustLeftPoint.position;
                        _chatIllust.rectTransform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                    }
                    break;
                case Pos.Center:
                    {
                        _chatIllust.rectTransform.position = _chatIllustCenterPoint.position;
                        _chatIllust.rectTransform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                    }
                    break;
                default:
                    break;
            }
        }

        private void DoChatEffect(IllustEffectType effectType, float duration)
        {
            if (duration <= 0.0f)
                return;

            switch (effectType)
            {
                case IllustEffectType.None:
                    break;
                case IllustEffectType.FadeIn:
                    {
                        _effectCoroutine = StartCoroutine(E_FadeIn(duration));
                        _isEffecting = true;
                    }
                    break;
                case IllustEffectType.FadeOut:
                    {
                        _effectCoroutine = StartCoroutine(E_FadeOut(duration));
                        _isEffecting = true;
                    }
                    break;
                case IllustEffectType.Oscillation:
                    {
                        _effectCoroutine = StartCoroutine(E_Shake(duration));
                        _isEffecting = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetChattingType(ChattingType type)
        {
            switch (type)
            {
                case ChattingType.Chatting:
                    {
                        _chattingBG.SetActive(true);
                        _chatIllust.gameObject.SetActive(true);
                        _chaterNamePanel.SetActive(true);
                        _ThinkingBG.SetActive(false);
                    }
                    break;
                case ChattingType.Thinking:
                    {
                        _chattingBG.SetActive(false);
                        _chatIllust.gameObject.SetActive(false);
                        _chaterNamePanel.SetActive(false);
                        _ThinkingBG.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }

        private IEnumerator E_FadeInCanvasGroup(float duration,Action OnFadeStart, Action OnFadeFinish)
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

        private IEnumerator E_FadeIn(float duration)
        {
            float timeMark = Time.time;
            float speed = 1.0f /duration;
            Color c = _chatIllust.color;
            c.a = 0;
            _chatIllust.color = c;
            while (c.a < 1)
            {
                c.a += (Time.time - timeMark) * speed;
                _chatIllust.color = c;
                yield return null;
            }
            _effectCoroutine = null;
            _isEffecting = false;
        }

        private IEnumerator E_FadeOut(float duration)
        {
            float timeMark = Time.time;
            float speed = 1.0f / duration;
            Color c = _chatIllust.color;
            c.a = 1;
            _chatIllust.color = c;
            while (c.a > 0)
            {
                c.a -= (Time.time - timeMark) * speed;
                _chatIllust.color = c;
                yield return null;
            }
            _effectCoroutine = null;
            _isEffecting = false;
        }

        private IEnumerator E_Shake(float duration)
        {
            Vector3 startPos = _chatIllust.transform.position;
            float amplitude = _chatIllust.transform.GetComponent<RectTransform>().rect.width / 40;
            float timeMark = Time.time;

            while (Time.time - timeMark < duration)
            {
                _chatIllust.transform.position = startPos + Random.insideUnitSphere * amplitude;
                yield return null;
            }
            _chatIllust.transform.position = startPos;
            _effectCoroutine = null;
            _isEffecting = false;
        }
    }
}
