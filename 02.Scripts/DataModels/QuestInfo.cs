using System.Collections.Generic;
using UnityEngine;
using HTH.IDs;
using HTH.DataStructures;
using UnityEditor;
using System.Diagnostics.Tracing;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 메인 퀘스트 데이터
    /// 수정자  : 권병석
    /// 수정일  : 2023_02_12
    /// 설명    : ScriptableObject 생성 메소드 추가
    /// </summary>
    [CreateAssetMenu(fileName = "new MainQuestInfo", menuName = "HomesToHolmes/Create MainQuestInfo")]
    public class QuestInfo : ScriptableObject
    {
        public RoomData roomData
            => JsonUtility.FromJson<RoomData>(Resources.Load<TextAsset>($"RoomData/Quest{id.value}").ToString());

        public QuestID id;
        public NPCID npcId;
        public string title;
        public string description;
        public Gold budget;
        public List<int> itemProvidedList;
        public List<int> itemShoppingList;
        public List<int> statsRequired;
        public List<int> hashtagsRequired;
        public Color colorRequired;
        public Gold rewardGold;
        public List<UKeyValuePair<int, int>> rewardItems_RankS;
        public List<UKeyValuePair<int, int>> rewardItems_RankA;
        public List<UKeyValuePair<int, int>> rewardItems_RankB;
        public List<UKeyValuePair<int, int>> rewardItems_RankC;
        public List<UKeyValuePair<int, int>> rewardItems_RankD;
        public List<UKeyValuePair<int, int>> rewardItems_RankE;
        public ChatsInfo chatsBeginning;
        public ChatsInfo chatsEnding;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public Gold GetRewardGoldByRank(Rank rank)
        {            
            switch (rank)
            {
                case Rank.S: return rewardGold * 1.0f;
                case Rank.A: return rewardGold * 0.7f;
                case Rank.B: return rewardGold * 0.5f;
                case Rank.C: return rewardGold * 0.3f;
                case Rank.D: return rewardGold * 0.2f;
                case Rank.E: return rewardGold * 0.15f;
                case Rank.F: return rewardGold * 0.1f;
                default: return rewardGold * 0.0f;
            }
        }

#if UNITY_EDITOR
        public static void CreateAssets(int id, string npcId, string title, string description, int[] budget,
                                        int[] itemProvidedList, int[] itemShoppingList, int[] statsRequired,
                                        int[] hashtagsRequired, string colorRequired, int[] rewardGold,
                                        int[] rewardItems_RankS, int[] rewardItems_RankA, int[] rewardItems_RankB,
                                        int[] rewardItems_RankC, int[] rewardItems_RankD, int[] rewardItems_RankE
                                        )
        {
            if (AssetDatabase.FindAssets($"Quest{id}", new string[] { $"Assets/08.Data/QuestInfo/" }).Length > 0)
            {
                Debug.LogWarning($"[QeustInfo] : Quest{id}가 이미 존재합니다");
                return;
            }

            string[] npcIdGUIDs = AssetDatabase.FindAssets(npcId, new string[] { $"Assets/08.Data/NPCID/" });
            if(npcIdGUIDs.Length <= 0)
            {
                Debug.LogWarning($"[QuestInfo] : {npcId}가 존재하지 않습니다.");
                return;
            }
            
            string[] chatsBeginningGUIDs = AssetDatabase.FindAssets($"Quest{id}", new string[] { $"Assets/08.Data/ChatsInfo/" });
            if(chatsBeginningGUIDs.Length <= 0)
            {
                Debug.LogWarning($"[QuestInfo] : Quest{id}의 ChatsInfo가 존재하지 않습니다.");
                return;
            }


            QuestInfo asset = ScriptableObject.CreateInstance<QuestInfo>();
            
            //npcID
            asset.npcId = AssetDatabase.LoadAssetAtPath<NPCID>(AssetDatabase.GUIDToAssetPath(npcIdGUIDs[0]));

            //chatsBeginning
            asset.chatsBeginning = AssetDatabase.LoadAssetAtPath<ChatsInfo>(AssetDatabase.GUIDToAssetPath(chatsBeginningGUIDs[0]));

            //title
            asset.title = title;

            //description
            asset.description = description;

            //QuestID
            QuestID newID = ScriptableObject.CreateInstance<QuestID>();
            newID.value = id;
            newID.tag = $"Quest{id}";
            AssetDatabase.CreateAsset(newID, $"Assets/08.Data/QuestID/Quest{id}.asset");
            asset.id = newID;

            //budget
            asset.budget = new Gold(budget[0], budget[1], budget[2], budget[3]);

            //itemProvidedList
            List<int> itemProvidedIDs = new List<int>();
            if (itemProvidedList[0] == 0)
            {
                asset.itemProvidedList = itemProvidedIDs;
            }
            else
            {
                for (int i = 0; i < itemProvidedList.Length; i++)
                {
                    itemProvidedIDs.Add(itemProvidedList[i]);
                }
                asset.itemProvidedList = itemProvidedIDs;
            }

            //itemShoppingList
            List<int> itemShoppingIDs = new List<int>();
            if (itemShoppingList[0] == 0)
            {
                asset.itemShoppingList = itemShoppingIDs;
            }
            else
            {
                for (int i = 0; i < itemShoppingList.Length; i++)
                {
                    itemShoppingIDs.Add(itemShoppingList[i]);
                }
                asset.itemShoppingList = itemShoppingIDs;
            }

            //statsRequired
            List<int> statsRequiredValue = new List<int>();
            for (int i = 0; i < statsRequired.Length; i++)
            {
                statsRequiredValue.Add(statsRequired[i]);
            }
            asset.statsRequired = statsRequiredValue;

            //hashtagsRequired
            List<int> hashtagsRequiredValue = new List<int>();
            for (int i = 0; i < hashtagsRequired.Length; i++)
            {
                hashtagsRequiredValue.Add(hashtagsRequired[i]);
            }
            asset.hashtagsRequired = hashtagsRequiredValue;

            //colorRequired
            Color color = new Color();
            if(ColorUtility.TryParseHtmlString(colorRequired, out color))
            {
                asset.colorRequired = color;
            }

            //rewardGold
            asset.rewardGold = new Gold(rewardGold[0], rewardGold[1], rewardGold[2], rewardGold[3]);

            //rewardItems_RankS
            List<UKeyValuePair<int, int>> rewardItemS = new List<UKeyValuePair<int, int>>();
            if (rewardItems_RankS[0] == 0) // 데이터가 없으면
            {
                asset.rewardItems_RankS = rewardItemS; // 빈 리스트로 넣음
            }
            else
            {
                for (int i = 0; i < rewardItems_RankS.Length; i++)
                {
                    rewardItemS.Add(new UKeyValuePair<int, int>(rewardItems_RankS[i], 1));
                }
                asset.rewardItems_RankS = rewardItemS;
            }

            //rewardItems_RankA
            List<UKeyValuePair<int, int>> rewardItemA = new List<UKeyValuePair<int, int>>();
            if(rewardItems_RankA[0] == 0) // 데이터가 없으면
            {
                asset.rewardItems_RankA = rewardItemA; // 빈 리스트로 넣음
            }
            else
            {
                for (int i = 0; i < rewardItems_RankA.Length; i++)
                {
                    rewardItemA.Add(new UKeyValuePair<int, int>(rewardItems_RankA[i], 1));
                }
                asset.rewardItems_RankA = rewardItemA;
            }

            //rewardItems_RankB
            List<UKeyValuePair<int, int>> rewardItemB = new List<UKeyValuePair<int, int>>();
            if (rewardItems_RankB[0] == 0) // 데이터가 없으면
            {
                asset.rewardItems_RankB = rewardItemB; // 빈 리스트로 넣음
            }
            else
            {
                for (int i = 0; i < rewardItems_RankB.Length; i++)
                {
                    rewardItemB.Add(new UKeyValuePair<int, int>(rewardItems_RankB[i], 1));
                }
                asset.rewardItems_RankB = rewardItemB;
            }

            //rewardItems_RankC
            List<UKeyValuePair<int, int>> rewardItemC = new List<UKeyValuePair<int, int>>();
            if (rewardItems_RankC[0] == 0) // 데이터가 없으면
            {
                asset.rewardItems_RankC = rewardItemC; // 빈 리스트로 넣음
            }
            else
            {
                for (int i = 0; i < rewardItems_RankC.Length; i++)
                {
                    rewardItemC.Add(new UKeyValuePair<int, int>(rewardItems_RankC[i], 1));
                }
                asset.rewardItems_RankC = rewardItemC;
            }

            //rewardItems_RankD
            List<UKeyValuePair<int, int>> rewardItemD = new List<UKeyValuePair<int, int>>();
            if (rewardItems_RankD[0] == 0) // 데이터가 없으면
            {
                asset.rewardItems_RankD = rewardItemD; // 빈 리스트로 넣음
            }
            else
            {
                for (int i = 0; i < rewardItems_RankD.Length; i++)
                {
                    rewardItemD.Add(new UKeyValuePair<int, int>(rewardItems_RankD[i], 1));
                }
                asset.rewardItems_RankD = rewardItemD;
            }

            //rewardItems_RankE
            List<UKeyValuePair<int, int>> rewardItemE = new List<UKeyValuePair<int, int>>();
            if (rewardItems_RankE[0] == 0) // 데이터가 없으면
            {
                asset.rewardItems_RankE = rewardItemE; // 빈 리스트로 넣음
            }
            else
            {
                for (int i = 0; i < rewardItems_RankE.Length; i++)
                {
                    rewardItemE.Add(new UKeyValuePair<int, int>(rewardItems_RankE[i], 1));
                }
                asset.rewardItems_RankE = rewardItemE;
            }
            
            AssetDatabase.CreateAsset(asset, $"Assets/08.Data/QuestInfo/Quest{id}.asset");
            AssetDatabase.SaveAssets();
            Debug.Log($"[QuestInfo] : Quest{id} 생성 완료.");
            EditorUtility.FocusProjectWindow();
            Selection.activeInstanceID = asset.GetInstanceID();
        }
#endif
    }
}