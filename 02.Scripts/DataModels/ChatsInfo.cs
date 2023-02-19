using HTH.DataModels;
using HTH.IDs;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 작성자  : 조영민
/// 작성일  : 2023_01_21
/// 설명    : NPC와의 채팅정보
/// </summary>
[CreateAssetMenu(fileName = "new ChatsInfo", menuName = "HomesToHolmes/Create ChatsInfo")]
public class ChatsInfo : ScriptableObject
{    
    public List<ChatData> chats;

#if UNITY_EDITOR
    public static void CreateAsset(string fileName, List<ChatData> data)
    {
        if (data.Count <= 0)
            return;

        if (AssetDatabase.FindAssets($"{fileName}", new string[] { $"Assets/08.Data/ChatsInfo/" }).Length > 0)
        {
            Debug.LogWarning($"[ChatsInfo] : {fileName} data is already exist.");
            return;
        }

        ChatsInfo asset = ScriptableObject.CreateInstance<ChatsInfo>();
        asset.chats = data;


        AssetDatabase.CreateAsset(asset, $"Assets/08.Data/ChatsInfo/{fileName}.asset");
        AssetDatabase.SaveAssets();
        Debug.Log($"[ChatsInfo] : {asset.chats[0].npcID.value} data is created.");
        EditorUtility.FocusProjectWindow();
        Selection.activeInstanceID = asset.GetInstanceID();
    }
#endif
}