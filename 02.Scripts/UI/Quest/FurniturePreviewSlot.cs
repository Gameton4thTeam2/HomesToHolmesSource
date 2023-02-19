using UnityEngine;
using UnityEngine.UI;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 가구 미리보기 슬롯
    /// </summary>
    public class FurniturePreviewSlot : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetUp(Sprite icon)
        {
            image.sprite = icon;
        }
    }
}