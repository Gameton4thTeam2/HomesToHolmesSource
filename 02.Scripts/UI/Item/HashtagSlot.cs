using UnityEngine;
using TMPro;
using HTH.DataModels;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_09
    /// 설명    : 해시태그슬롯. 글자수에따라 너비 조정
    /// </summary>
    public class HashtagSlot : MonoBehaviour
    {
        [HideInInspector] public int hashtagIndex;
        [SerializeField] private TMP_Text _tag;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private float _fontWidth = 20.0f;
        [SerializeField] private GameObject _okMark;

        public void Show(int hashtagIndex, bool isOK)
        {
            this.hashtagIndex = hashtagIndex;
            _tag.text = $"#{HashtagAssets.instance[hashtagIndex].tag}";
            _rect.sizeDelta = new Vector2(_fontWidth * _tag.text.Length, _rect.sizeDelta.y);
            _okMark.SetActive(isOK);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
