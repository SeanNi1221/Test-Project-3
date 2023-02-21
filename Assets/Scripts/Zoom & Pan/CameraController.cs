using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //To record camera state at a specific moment
    private class CameraState {
        public readonly float Fov;
        public readonly Matrix4x4 Projection;
        public readonly Plane[] Frustrum;
        public readonly float viewSize;
        public CameraState(Camera camera) {
            Fov = camera.fieldOfView;
            Projection = camera.projectionMatrix;
            Frustrum = GeometryUtility.CalculateFrustumPlanes(camera);
            viewSize = Mathf.Tan(Mathf.Deg2Rad * Fov/2);
        }
    }
    //To constrain pan and zoom inside a rect area
    private class Canvas {
        private CameraController _controller;
        public float Scale { get; private set; }
        public float XMin { get; private set; }
        public float XMax { get; private set; }
        public float YMin { get; private set; }
        public float YMax { get; private set; }

        public Canvas(CameraController controller) {
            _controller = controller;
            Scale = 1;
        }

        //The canvas size is determined by ( orgin view size / current view size)
        public void UpdateBy(Camera camera, CameraState origin) {
            var currentViewSize = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView/2);
            Scale = origin.viewSize/currentViewSize;
            var expand = Scale - 1;
            XMin = YMin = -expand;
            XMax = YMax = expand;
        }
    }
    private static class InputProcessor {
        public static readonly float ScrollWheel = Time.deltaTime *
# if UNITY_WEBGL
        250;
# else
        250;
#endif
        public static readonly float MouseX = Time.deltaTime *
# if UNITY_WEBGL
        1;
# else
        5;
#endif
        public static readonly float MouseY = Time.deltaTime *
# if UNITY_WEBGL
        1;
# else
        5;
#endif
    }
    [Range(0,10)] public float ZoomSpeed = 1;
    [Range(0,10)] public float PanSeedX = 1;
    [Range(0,10)] public float PanSeedY = 1;
    public bool IsZooming => _zoomDelta != 0;
    public bool IsPanning => _panDelta != Vector2.zero;
    private float _zoomDelta;
    private Vector2 _panDelta;
    //The state when there's no pan, no zoom applied
    private CameraState _origin;
    private CameraController.Canvas _canvas;
    //Has the camera been touched during the current frame?
    private bool _isDirty;
    [SerializeField] private Camera _camera;
    private float _lensShiftX;
    private float _lensShiftY;

    void OnValidate() {
        if (!_camera) _camera = Camera.main;
    }

    void Start() {
        SetOrigin();
        ResetCanvas();
    }
    void Update()
    {
        GetInputs();

        if (IsZooming) Zoom(_zoomDelta);
        if (IsPanning) Pan(_panDelta);

        if (_isDirty) UpdateProjectionMatrix();
        _isDirty = false;
    }
    //Set the current camera state as origin
    internal void SetOrigin() {
        _origin = new CameraState(_camera);
    }
    internal void ResetCanvas() {
        _canvas = new Canvas(this);
    }
    internal void Zoom(float delta) {
        if (delta < 0 && _camera.fieldOfView >= _origin.Fov) return;

        float newFov = _camera.fieldOfView - delta;
        _camera.fieldOfView = Mathf.Clamp(newFov, 10, _origin.Fov);

        //The scale of the canvas changes with fov, therefore a compensation
        //lens shift is needed after changing the fov to ensure the view center
        //is not changed.
        float scale0 = _canvas.Scale;
        _canvas.UpdateBy(_camera, _origin);
        float compensation = _canvas.Scale/scale0;

        _lensShiftX *= compensation;
        _lensShiftY *= compensation;

        _lensShiftX = Mathf.Clamp(_lensShiftX, _canvas.XMin, _canvas.XMax);
        _lensShiftY = Mathf.Clamp(_lensShiftY, _canvas.YMin, _canvas.YMax);

        _isDirty = true;
    }
    internal void Pan(Vector2 delta) {
        float newShiftX = _lensShiftX - delta.x;
        _lensShiftX = Mathf.Clamp(newShiftX, _canvas.XMin, _canvas.XMax);

        float newShiftY = _lensShiftY - delta.y;
        _lensShiftY = Mathf.Clamp(newShiftY, _canvas.YMin, _canvas.YMax);

        _isDirty = true;
    }
    private void GetInputs() {
        var zoomInput = Input.GetAxis("Mouse ScrollWheel");
        _zoomDelta = zoomInput == 0 ? 0 : zoomInput * ZoomSpeed * InputProcessor.ScrollWheel;

        var panInput = Input.GetButton("Fire1") ?
            new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) :
            Vector2.zero;
        _panDelta = new Vector2(
            panInput.x == 0 ? 0 : panInput.x * PanSeedX * InputProcessor.MouseX,
            panInput.y == 0 ? 0 : panInput.y * PanSeedY * InputProcessor.MouseY
        );
    }

    //After setting _camera.projectionMatrix, the camera no longer updates its rendering
    //based on its fieldOfView. This lasts until we call ResetProjectionMatrix.
    //Hence, we have to apply the field of view manually by re-constructing the matrix.
    private void UpdateProjectionMatrix() {
        //Apply field of view
        var m = Matrix4x4.Perspective(
            _camera.fieldOfView,
            _camera.aspect,
            _camera.nearClipPlane,
            _camera.farClipPlane
        );
        //Apply lens shift
        m[0, 2] = _lensShiftX;
        m[1, 2] = _lensShiftY;
        _camera.projectionMatrix = m;
    }
}
