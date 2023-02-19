using UnityEngine;
using System.Collections.Generic;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 월드맵의 빌딩들 관리 클래스
    /// </summary>
    public class BuildingManager : SingletonMonoBase<BuildingManager>
    {
        [HideInInspector] public Dictionary<int, Building> buildings = new Dictionary<int, Building>();
        [HideInInspector] public Building playerBuilding;

        public void RegisterBuilding(Building building)
        {
            if (buildings.TryAdd(building.id.value, building) == false)
                Debug.LogError($"[BuildingManager] : Failed to register {building.id}. It is already exist.");

            Debug.Log($"[BuildingManager] : Building {building.id} registered. ");

            if (building.gameObject.tag == "Player")
                playerBuilding = building;
        }
    }
}