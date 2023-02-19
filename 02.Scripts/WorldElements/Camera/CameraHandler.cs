using System.Threading;
using UnityEngine;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 카메라 시점
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraHandler : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Move,
            Zoom
        }
        private State _state;

        public static CameraHandler instance;
        private Camera _cam;
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _defaultPosition = new Vector3(-10.0f, 13.0f, -10.0f);
        [SerializeField] private Quaternion _defaultRotation = Quaternion.Euler(35.0f, 45.0f, 0.0f);
        [SerializeField] private float _defaultDistance = Vector3.Distance(Vector3.zero, new Vector3(-10.0f, 13.0f, -10.0f));
        [SerializeField] private float _defaultOrthographicSize = 11.0f;
        private Vector3 _positionOffset = Vector3.up * 2.0f;
        private float _distance;
        private float _orthographicSize;
        private Vector3 _prevPosition;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void SetTarget(Transform target)
        {
            _target = target;
            SetDefault();
        }

        public void SetDefault()
        {
            _orthographicSize = _defaultOrthographicSize;
            _distance = _defaultDistance;
            transform.position = _target.position;
            transform.rotation = _defaultRotation;
            transform.Translate(Vector3.back * _distance + _positionOffset);
        }

        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void Awake()
        {
            instance = this;
            _cam = GetComponent<Camera>();
        }

        private void Start()
        {
            SetDefault();
        }

        private void Update()
        {
            if (_target == null)
                return;

            Rotate();
            //Zoom();
        }

        private void Rotate()
        {
//#if UNITY_ANDROID
//            if (Input.touchCount != 1)
//                return;
//#endif
            if (Input.GetMouseButtonDown(0))
            {
                _prevPosition = _cam.ScreenToViewportPoint(Input.mousePosition);
                _state = State.Move;
            }
            else if (_state == State.Move &&
                     Input.GetMouseButton(0))
            {
                Vector3 newPosition = _cam.ScreenToViewportPoint(Input.mousePosition);
                Vector3 direction = _prevPosition - newPosition;

                float rotationAroundYAxis = -direction.x * 180.0f;
                float rotationAroundXAxis = direction.y * 180.0f;

                transform.position = _target.position;
                transform.Rotate(Vector3.right, rotationAroundXAxis);
                transform.Rotate(Vector3.up, rotationAroundYAxis);
                transform.Translate(Vector3.back * _distance + _positionOffset);

                _prevPosition = newPosition;
            }
        }

        private void Zoom()
        {
//#if UNITY_ANDROID
//            if (Input.touchCount != 2)
//                return;
//#endif
            _state = State.Zoom;

            float disX = 0.0f;
            float disY = 0.0f;

            disX = Mathf.Abs(Input.GetTouch(0).deltaPosition.x - Input.GetTouch(1).deltaPosition.x);
            disY = Mathf.Abs(Input.GetTouch(0).deltaPosition.y - Input.GetTouch(1).deltaPosition.y);
            _cam.orthographicSize += (disX + disY);
        }

    }
}
