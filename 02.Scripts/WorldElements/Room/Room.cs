using System.Collections.Generic;
using UnityEngine;
using HTH.IDs;
using HTH.DataModels;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템들로 꾸밀 수 있는 방
    /// </summary>
    public class Room : MonoBehaviour
    {
        public RoomID id;
        public List<Item> items = new List<Item>();
        public Stats stats;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public static Room Create(int id, Vector3 position)
        {
            return Instantiate(RoomAssets.instance[id], position, Quaternion.identity);
        }

        /// <summary>
        /// RoomData 로 Room 을 만드는 팩토리
        /// </summary>
        public static Room Create(RoomData data, Vector3 position)
        {
            Room room = Instantiate(RoomAssets.instance[data.id], position, Quaternion.identity);
            GameObject go;
            foreach (ItemData itemData in data.items)
            {
                go = Instantiate(ItemAssets.instance[itemData.id].prefab, room.transform);
                go.transform.localPosition = itemData.position;
                go.transform.localRotation = itemData.rotation;
                room.AddItem(go.GetComponent<Item>());
            }
            return room;
        }

        public static async UniTask<Room> CreateAsync(RoomData data, Vector3 position)
        {
            Room result = null;
            Dummy dummy = new GameObject().AddComponent<Dummy>();
            dummy.gameObject.name = $"Room{data.id}";
            dummy.InstantiateGameObjectInCoroutine<Room>(RoomAssets.instance[data.id],
                                                         position,
                                                         (room) =>
                                                         {
                                                             result = room;
                                                             room.StartCoroutine(room.E_Init(data));
                                                         });
            await UniTask.WaitUntil(() => result != null);
            return result;

        }

        IEnumerator E_Init(RoomData data) 
        {
            yield return new WaitUntil(() => ItemAssets.instance != null);

            GameObject item;
            foreach (ItemData itemData in data.items)
            {
                item = Instantiate(ItemAssets.instance[itemData.id].prefab, transform);
                item.transform.localPosition = itemData.position;
                item.transform.localRotation = itemData.rotation;
                AddItem(item.GetComponent<Item>());
            }
            yield return null;
        }


        public void AddItem(Item item)
        {
            items.Add(item);
            foreach (var modifier in item.statModifiers)
            {
                AddStatModifier(modifier);
            }
        }

        public bool RemoveItem(Item item)
        {
            if (items.Remove(item))
            {
                foreach (var modifier in item.statModifiers)
                {
                    RemoveStatModifier(modifier);
                }

                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public void SaveRoomData()
        {
            Item[] items = transform.GetComponentsInChildren<Item>();
            RoomData roomData = new RoomData();
            roomData.id = id.value;
            roomData.items = items.Select(x => new ItemData()
            {
                id = x.id.value,
                position = x.transform.localPosition,
                rotation = x.transform.localRotation
            }).ToList();

            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save room data", "new Room Data", "json", "", $"Resources/RoomData");
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(roomData));
        }
#endif

        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void AddStatModifier(StatModifier modifier)
        {
            stats[modifier.id].AddModifier(modifier);
        }

        private void RemoveStatModifier(StatModifier modifier)
        {
            stats[modifier.id].RemoveModifier(modifier);
        }

    }
}
