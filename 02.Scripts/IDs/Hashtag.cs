using UnityEditor;
using UnityEngine;

namespace HTH.IDs
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 검색 필터링용 해시태그
    /// </summary>
    [CreateAssetMenu(fileName = "new Hashtag", menuName = "HomesToHolmes/Create Hashtag")]
    public class Hashtag : ScriptableObject
    {
        public int index;
        public string tag;

#if UNITY_EDITOR
        public static void CreateAssets(int total)
        {
            Hashtag hashtag;
            for (int i = 0; i < total; i++)
            {
                hashtag = ScriptableObject.CreateInstance<Hashtag>();
                hashtag.index = i;
                hashtag.name = hashtag.tag = $"HASHTAG_NAME_{i}";
                AssetDatabase.CreateAsset(hashtag, $"Assets/08.Data/Hashtag/{hashtag.name}.asset");
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}
