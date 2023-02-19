using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 삭제 예정
    /// </summary>
    [Obsolete]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ItemBuilder : MonoBehaviour
    {
        private Item _item;
        private LayerMask _targetLayer;
        private Camera _cam;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        [SerializeField] private GameObject _sizeCube;
        [SerializeField] private Transform _gridPlane;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Handle(Item item)
        {
            _item = item;
            _meshFilter.sharedMesh = item.GetComponent<MeshFilter>().sharedMesh;
            _meshRenderer.sharedMaterial = item.GetComponent<MeshRenderer>().sharedMaterial;
            _meshRenderer.sharedMaterial.color = new Color(_meshRenderer.sharedMaterial.color.r,
                                                           _meshRenderer.sharedMaterial.color.g,
                                                           _meshRenderer.sharedMaterial.color.b,
                                                           0.5f);
            _sizeCube.transform.localScale = item.GetComponent<BoxCollider>().size * 1.1f;
            _sizeCube.transform.localPosition = item.GetComponent<BoxCollider>().center;
            _gridPlane.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Cancel()
        {
            _item = null;
            _gridPlane.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _cam = Camera.main;
            _targetLayer = 1 << LayerMask.NameToLayer("Item") | 1 << LayerMask.NameToLayer("Ground");
            gameObject.SetActive(false);
        }

        private void Update()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _targetLayer))
            {
                transform.position = WorldInfo.GetAdjustedPosToGrid(hit.point, Axis.X | Axis.Z);
                float x = (_sizeCube.transform.localScale.x - WorldInfo.GridDistance) % 1.0 == 0.0f ? 0.0f : 0.25f;
                float z = (_sizeCube.transform.localScale.z - WorldInfo.GridDistance) % 1.0 == 0.0f ? 0.0f : 0.25f;
                transform.position += new Vector3(x, 0.0f, z);
                _gridPlane.position = new Vector3(_gridPlane.position.x,
                                                  transform.position.y,
                                                  _gridPlane.position.z);
            }
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButton(0))
                Instantiate(_item, transform.position, Quaternion.identity);

            Cancel();
        }
    }
}