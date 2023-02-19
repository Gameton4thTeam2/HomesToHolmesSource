using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 랜덤 아이템 생성 박스 (테스트용)
    /// </summary>
    public class ItemBox : MonoBehaviour
    {
        [SerializeField] private List<Item> _items = new List<Item>();
        [SerializeField] private Vector3 _popUpOffset;
        [SerializeField] private float _popUpSpeed;
        [SerializeField] private ParticleSystem _popUpEffect;
        private Queue<Item> _itemsQueue;
        private bool _poping;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void PopUpItem()
        {
            if (_itemsQueue.Count <= 0)
                return;

            Transform item = Instantiate(_itemsQueue.Dequeue(), transform.position, Quaternion.identity).transform;
            StartCoroutine(E_PopUp(item));
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _itemsQueue = new Queue<Item>(_items);
        }

        IEnumerator E_PopUp(Transform item)
        {
            _poping = true;

            float elapsedTime = 0.0f;
            while (elapsedTime * _popUpSpeed <= 1.0f)
            {
                if (elapsedTime * _popUpSpeed > 0.3f &&
                    _popUpEffect.isPlaying == false)
                    _popUpEffect.Play();

                item.localScale = Vector3.one * elapsedTime * _popUpSpeed;
                item.position = Vector3.Lerp(transform.position, transform.position + _popUpOffset, elapsedTime * _popUpSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            item.localScale = Vector3.one;
            item.position = transform.position + _popUpOffset;
            _poping = false;
        }

        private void OnMouseDown()
        {
            if (_poping)
                return;

            PopUpItem();
        }
    }
}
