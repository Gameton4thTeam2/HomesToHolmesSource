using System.Collections;
using UnityEngine;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 선으로된 큐브 렌더러.. 아직 모든선을 다 그려놓지는 않았음. 선 추가해야함.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class WireCube : MonoBehaviour
    {
        private LineRenderer _lineRenderer;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Draw(Vector3 position, Vector3 center, Vector3 angle, Vector3 size)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(angle);
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, new Vector3(size.x / 2.0f, -size.y / 2.0f, -size.z / 2.0f) + center);
            _lineRenderer.SetPosition(1, new Vector3(size.x / 2.0f, size.y / 2.0f, -size.z / 2.0f) + center);
            _lineRenderer.SetPosition(2, new Vector3(-size.x / 2.0f, size.y / 2.0f, -size.z / 2.0f) + center);
            _lineRenderer.SetPosition(3, new Vector3(-size.x / 2.0f, -size.y / 2.0f, -size.z / 2.0f) + center);
            _lineRenderer.SetPosition(4, new Vector3(size.x / 2.0f, -size.y / 2.0f, -size.z / 2.0f) + center);
            _lineRenderer.SetPosition(5, new Vector3(size.x / 2.0f, -size.y / 2.0f, size.z / 2.0f) + center);
            _lineRenderer.SetPosition(6, new Vector3(-size.x / 2.0f, -size.y / 2.0f, size.z / 2.0f) + center);
            _lineRenderer.SetPosition(7, new Vector3(-size.x / 2.0f, -size.y / 2.0f, -size.z / 2.0f) + center);
            _lineRenderer.SetPosition(8, new Vector3(-size.x / 2.0f, size.y / 2.0f, -size.z / 2.0f) + center);
            _lineRenderer.SetPosition(9, new Vector3(-size.x / 2.0f, size.y / 2.0f, size.z / 2.0f) + center);
            _lineRenderer.SetPosition(10, new Vector3(-size.x / 2.0f, -size.y / 2.0f, size.z / 2.0f) + center);
        }

        public void Hide()
        {
            _lineRenderer.enabled = false;
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
    }
}
