using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_10
    /// 설명    : CanvasGroup을 터치해서 하이라이팅함
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class HighlightingCanvasGroup : MonoBehaviour, IPointerDownHandler
    {
        private CanvasGroup _canvasGroup;
        [SerializeField] private float _alphaNormal = 0.3f;
        [SerializeField] private float _alphaHighlighted = 1.0f;
        [SerializeField] private float _fadeDelta = 0.05f;
        [SerializeField] private float _duration = 2.0f;
        private bool _isBusy = false;
        private bool _isCoroutineOn = false;
        private Coroutine _coroutine = null;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void OnPointerDown(PointerEventData eventData)
        {
            Highlight();
        }

        public void Highlight()
        {
            if (_isBusy)
                return;

            if (_isCoroutineOn)
                StopCoroutine(_coroutine);

            _isBusy = true;
            _isCoroutineOn = true;
            _coroutine = StartCoroutine(E_Highlight());
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private IEnumerator E_Highlight()
        {
            while (_canvasGroup.alpha < _alphaHighlighted)
            {
                _canvasGroup.alpha += _fadeDelta;
                yield return null;  
            }

            yield return new WaitForSeconds(_duration);
            _isBusy = false;

            while (_canvasGroup.alpha > _alphaNormal)
            {
                _canvasGroup.alpha -= _fadeDelta;
                yield return null;
            }

            _coroutine = null;
            _isCoroutineOn = false;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = _alphaNormal;
        }

        private void OnEnable()
        {
            Highlight();
        }

        private void OnDisable()
        {
            _isBusy = false;
            _isCoroutineOn = false;
        }
    }
}
