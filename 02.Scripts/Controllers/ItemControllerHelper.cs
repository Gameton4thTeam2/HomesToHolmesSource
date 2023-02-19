using UnityEngine;
using HTH.WorldElements;

namespace HTH.InputHandlers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 유저가 아이템을 핸들링 할 때 부수적으로 도움을주는 클래스. 그리드 / 아웃라인 등의 렌더링
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ItemControllerHelper : SingletonMonoBase<ItemControllerHelper>
    {
        public bool isActivated;
        private Transform _target;
        private MeshFilter _meshFilter;
        private LineRenderer _lineRenderer;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void RefreshMaterial(bool isValid)
        {
            gameObject.layer = isValid ? 29 : 30;
        }

        public void Active(Transform target)
        {
            if (target == null)
                return;

            _meshFilter.mesh = target.GetComponent<MeshFilter>().sharedMesh;
            transform.position = target.transform.position;
            _target = target;
            isActivated = true;
        }

        public void Deactive(Transform target)
        {
            transform.position = new Vector3(-5000.0f, -5000.0f, -5000.0f);
            _target = null;
            isActivated = false;
        }

        public void DrawWireCube(Vector3 position, Vector3 center, Vector3 angle, Vector3 size)
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


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override protected void Awake()
        {
            base.Awake();
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Update()
        {
            if (isActivated)
            {
                if (_target == null)
                {
                    Deactive(null);
                }
                else
                {
                    transform.position = _target.transform.position;
                    transform.rotation = _target.transform.rotation;
                }
            }
        }        
    }
}