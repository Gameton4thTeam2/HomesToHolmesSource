using System;
using UnityEngine;

namespace HTH.DataModels
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 아이템 위치 및 각도를 데이터로 다루기위한 클래스
    /// </summary>
    [Serializable]
    public class ItemData
    {
        public int id;
        public Vector3 position;
        public Quaternion rotation;
    }
}
