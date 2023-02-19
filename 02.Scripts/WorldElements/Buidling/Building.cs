using HTH.IDs;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : NPC 가 있는 건물
    /// </summary>
    public class Building : MonoBehaviour
    {
        public BuildingID id;
        public List<Room> rooms;

        private void Awake()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => BuildingManager.instance != null);
                BuildingManager.instance.RegisterBuilding(this);
            });
        }

        private void OnMouseDown()
        {
            Player.instance.MoveTo(this);
        }
    }
}