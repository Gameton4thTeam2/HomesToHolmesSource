using HTH;
using HTH.WorldElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_27
    /// 설명    : 아이템의 그리드스내핑을 도와줌
    /// </summary>
    public class GridSnappingHelper : SingletonMonoBase<GridSnappingHelper>
    {
        public bool freeMove;
        public float gridDistance = 0.5f;
        private Transform _item;
        private Transform _room;

        public void GridScaleUp()
        {
            transform.localScale *= 2.0f;
            gridDistance *= 2.0f;
        }

        public void GridScaleDown()
        {
            transform.localScale /= 2.0f;
            gridDistance /= 2.0f;
        }

        public void SetRoom(Transform room)
        {
            _room = room;
            transform.localScale = Vector3.one / 2.0f;
            gridDistance = 0.5f / 2.0f;
        }

        public void SetTarget(Transform item)
        {
            _item = item;
            gameObject.SetActive(true);
        }


        private void Update()
        {
            if (_room == null ||
                _item == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                transform.position = _item.position;
                transform.rotation = _room.rotation;
                transform.localPosition += Vector3.up * 0.0001f;
            }
        }

        public Vector3 GetAdjustedPosToGrid(Vector3 pos, Axis axes)
        {
            if (freeMove)
            {
                return pos;
            }

            pos = Quaternion.Inverse(transform.rotation) * pos;

            if ((axes & Axis.X) == Axis.X)
            {
                pos.x = Mathf.Round((pos.x / gridDistance)) * gridDistance;
            }

            if ((axes & Axis.Y) == Axis.Y)
            {
                pos.y = Mathf.Round((pos.y / gridDistance)) * gridDistance;
            }

            if ((axes & Axis.Z) == Axis.Z)
            {
                pos.z = Mathf.Round((pos.z / gridDistance)) * gridDistance;
            }

            pos = transform.rotation * pos;

            return pos;
        }
    }
}