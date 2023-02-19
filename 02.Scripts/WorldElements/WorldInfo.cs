using System;
using UnityEngine;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 월드 맵 관련 글로벌 상수
    /// 삭제예정
    /// </summary>
    [Obsolete]
    public static class WorldInfo
    {
        public const float GridDistance = 0.5f;

        public static Vector3 GetAdjustedPosToGrid(Vector3 pos, Axis axes)
        {
            if ((axes & Axis.X) == Axis.X)
            {
                float xErr = pos.x % WorldInfo.GridDistance;
                //pos.x = xErr < WorldInfo.GridDistance / 2.0f ? pos.x - xErr : pos.x + xErr;
                pos.x -= xErr;
            }

            if ((axes & Axis.Y) == Axis.Y)
            {
                float yErr = pos.y % WorldInfo.GridDistance;
                //pos.y = yErr < WorldInfo.GridDistance / 2.0f ? pos.y - yErr : pos.y + yErr;
                pos.y -= yErr;
            }

            if ((axes & Axis.Z) == Axis.Z)
            {
                float zErr = pos.z % WorldInfo.GridDistance;
                //pos.z = zErr < WorldInfo.GridDistance / 2.0f ? pos.z - zErr : pos.z + zErr;
                pos.z -= zErr;
            }

            return pos;
        }
    }
}