using HTH.UnityAPIWrappers;
using HTH.WorldElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTH.InputHandlers
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_11
    /// 설명    : 카메라 컨트롤러
    /// </summary>
    public class CameraController : MonoBehaviour, IController
    {
        public static CameraController instance;
        [SerializeField] private CustomStandaloneInputModule _inputModule;
        [SerializeField] private float _sensitivity = 100.0f;
        private Camera _mainCam;
        public Camera worldCam;
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _defaultPosition = new Vector3(-10.0f, 13.0f, -10.0f);
        [SerializeField] private Quaternion _defaultRotation = Quaternion.Euler(35.0f, 45.0f, 0.0f);
        [SerializeField] private float _defaultDistance = Vector3.Distance(Vector3.zero, new Vector3(-10.0f, 13.0f, -10.0f));
        [SerializeField] private float _defaultOrthographicSize = 11.0f;
        private float _distance;
        private Vector3 _prevPosition;
        private float _prevDistance;
        [SerializeField] private CustomStandaloneInputModule _standaloneInputModule;

        public bool controllable
        {
            get
            {
                return _controllable;
            }
            set
            {
                if (value)
                {
                    _prevPosition = _mainCam.ScreenToViewportPoint(Input.mousePosition);
                    _prevDistance = 0.0f;
                }

                _controllable = value;
            }
        }
        private bool _controllable;


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
            worldCam.orthographicSize = _defaultOrthographicSize;
            _distance = _defaultDistance;
            transform.position = _target.position;
            transform.rotation = _defaultRotation;
            transform.Translate(Vector3.back * _distance);
        }

        public bool RequestControl()
        {
            return ControllerManager.instance.HandOverControlTo(this);
        }

        public bool RequestReturn()
        {
            return ControllerManager.instance.ReturnControl(this);
        }

        private void LateUpdate()
        {
            if (controllable)
            {
                if (_target == null)
                {
                    RequestReturn();
                }

                Control();
            }

            worldCam.orthographicSize -= Input.mouseScrollDelta.y * _sensitivity / 1000.0f;
        }

        private void Control()
        {            
            if (Input.GetMouseButton(0))
            {
#if UNITY_EDITOR
                if (_standaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(1 << LayerMask.NameToLayer("CastIgnoringUI"),
                                                                                     StandaloneInputModule.kMouseLeftId,
                                                                                     false))
                {
                    RequestReturn();
                    return;
                }
#elif UNITY_ANDROID
                if (Input.touchCount > 0 &&
                    _standaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(1 << LayerMask.NameToLayer("CastIgnoringUI"),
                                                                                     Input.GetTouch(0).fingerId,
                                                                                     false))
                {
                    RequestReturn();
                    return;
                }
#endif


#if UNITY_EDITOR
                Vector3 newPosition = _mainCam.ScreenToViewportPoint(Input.mousePosition);
                transform.RotateAround(_target.position, Vector3.right, -(newPosition.y - _prevPosition.y) * _sensitivity);
                transform.RotateAround(_target.position, Vector3.up, (newPosition.x - _prevPosition.x) * _sensitivity);
                transform.LookAt(_target.position);
                _prevPosition = newPosition;

#elif UNITY_ANDROID
                if (Input.touchCount == 1)
                {
                    Vector3 newPosition = _mainCam.ScreenToViewportPoint(Input.mousePosition);
                    transform.RotateAround(_target.position, Vector3.right, -(newPosition.y - _prevPosition.y) * _sensitivity);
                    transform.RotateAround(_target.position, Vector3.up, (newPosition.x - _prevPosition.x) * _sensitivity);
                    transform.LookAt(_target.position);
                    _prevPosition = newPosition;
                }
                else if (Input.touchCount == 2)
                {
                    float newDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    if (_prevDistance <= 0.0f)
                        _prevDistance = newDistance;

                    worldCam.orthographicSize -= (newDistance - _prevDistance) * _sensitivity / 10000.0f;
                    worldCam.orthographicSize = Mathf.Clamp(worldCam.orthographicSize, 3.0f, 15.0f);
                    _prevDistance = newDistance;
                }
#endif
            }
            else
            {
                RequestReturn();
            }
        }


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        private void Awake()
        {
            instance = this;
            _mainCam = Camera.main;
        }

        private void Start()
        {
            SetDefault();
        }
    }
}