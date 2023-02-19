using System.Collections.Generic;
using UnityEngine;
using HTH.IDs;
using HTH.DataStructures;
using HTH.WorldElements;
using UnityEditor;
using System.Linq;
using System;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 정보 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "new ItemInfo", menuName = "HomesToHolmes/Create ItemInfo")]
    public class ItemInfo : ScriptableObject
    {
        [Flags]
        public enum OptionFlags
        {
            None,
            StackOther,
            HangOnWall,
            AsWall
        }
        public ItemID id;
        new public string name => id.tag;
        public string description;
        public OptionFlags options;
        public List<Hashtag> hashtags;
        public Stats stats;
        public Gold buyPrice;
        public Gold sellPrice;
        public Rarity rarity;
        public Sprite icon;
        public GameObject prefab;

#if UNITY_EDITOR
        public static void CreateAsset(int id, 
                                       string name,
                                       string description,
                                       int optionFlags,
                                       int[] hashtags,
                                       int[] stats,
                                       int[] buyPrice,
                                       int[] sellPrice,
                                       int rarity)
        {
            if (AssetDatabase.FindAssets($"{name}", new string[] { $"Assets/08.Data/ItemInfo/" }).Length > 0)
            {
                Debug.LogWarning($"[ItemInfo] : {name} data is already exist.");
                return;
            }
            string[] iconGUIDs = AssetDatabase.FindAssets($"{name}", new string[] { "Assets/06.Textures/Items" });
            if (iconGUIDs.Length <= 0)
            {
                Debug.LogWarning($"[ItemInfo] : Failed to created {name} data because sprite doesn't exist");
                return;
            }
            string[] prefabGUIDs = AssetDatabase.FindAssets($"{name}", new string[] { "Assets/03.Prefabs/Items" });
            if (prefabGUIDs.Length <= 0)
            {
                Debug.LogWarning($"[ItemInfo] : Failed to created {name} data because prefab doesn't exist");
                return;
            }


            ItemInfo asset = ScriptableObject.CreateInstance<ItemInfo>();

            // icon
            asset.icon = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(iconGUIDs[0]));

            // prefab
            asset.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabGUIDs[0]));
            
            // id
            ItemID newID = ScriptableObject.CreateInstance<ItemID>();
            newID.value = id;
            newID.tag = name;
            AssetDatabase.CreateAsset(newID, $"Assets/08.Data/ItemID/ID_{name}.asset");
            asset.id = newID;
            
            // description
            asset.description = description;

            // options
            asset.options = (OptionFlags)optionFlags;

            // hashtags
            List<Hashtag> hashtagAssets = new List<Hashtag>();
            string[] hashtagGUIDs = AssetDatabase.FindAssets("t:Hashtag", new string[] { "Assets/08.Data/Hashtag" });
            for (int i = 0; i < hashtagGUIDs.Length; i++)
            {
                hashtagAssets.Add(AssetDatabase.LoadAssetAtPath<Hashtag>(AssetDatabase.GUIDToAssetPath(hashtagGUIDs[i])));
            }

            asset.hashtags = new List<Hashtag>();
            for (int i = 0; i < hashtags.Length; i++)
            {
                asset.hashtags.Add(hashtagAssets.Find(x => x.index == hashtags[i]));
            }

            //stats
            List<StatID> statIDs = new List<StatID>();
            string[] statIDGUIDs = AssetDatabase.FindAssets("t:StatID", new string[] { "Assets/08.Data/StatID" });
            for (int i = 0; i < statIDGUIDs.Length; i++)
            {
                statIDs.Add(AssetDatabase.LoadAssetAtPath<StatID>(AssetDatabase.GUIDToAssetPath(statIDGUIDs[i])));
            }
            Stat tmpStat;
            asset.stats = new Stats();
            asset.stats.list = new List<Stat>();
            for (int i = 0; i < stats.Length; i++)
            {
                tmpStat = new Stat();
                tmpStat.id = statIDs.Find(x => x.value == i);
                tmpStat.value = stats[i];
                asset.stats.list.Add(tmpStat);
            }

            // buy price
            asset.buyPrice = new Gold(buyPrice[0], buyPrice[1], buyPrice[2], buyPrice[3]);

            // sell price
            asset.sellPrice = new Gold(sellPrice[0], sellPrice[1], sellPrice[2], sellPrice[3]);

            // rarity
            asset.rarity = (Rarity)rarity;

            // Add item component to prefab
            Item item = null;
            if (asset.prefab.TryGetComponent(out item) == false)
            {
                item = asset.prefab.AddComponent<Item>();
            }
            item.id = newID;

            AssetDatabase.CreateAsset(asset, $"Assets/08.Data/ItemInfo/{name}.asset");
            AssetDatabase.SaveAssets();
            Debug.Log($"[ItemInfo] : {name} data is created.");
            EditorUtility.FocusProjectWindow();
            Selection.activeInstanceID = asset.GetInstanceID();
        }
#endif

    }
}