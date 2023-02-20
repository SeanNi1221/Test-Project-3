using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ZoomPan : MonoBehaviour
{
    [Serializable]
    public class State {
        public Camera Camera { get; private set; }
        public float FovVertical { get; private set; }
        public float FovHorizontal { get; private set; }
        public Quaternion Rotation { get; private set; }
        public float XMin { get; private set; }
        public float XMax { get; private set; }
        public float YMin { get; private set; }
        public float YMax { get; private set; }
        public State(Camera camera) {
            Camera = camera;
        }
        internal void Set() {
            FovVertical = Camera.fieldOfView;
            FovHorizontal = Camera.VerticalToHorizontalFieldOfView(FovVertical, Camera.aspect);
            Rotation = Camera.transform.rotation;
            XMin = -FovHorizontal/2f;
            XMax = FovHorizontal/2f;
            YMin = -FovVertical/2f;
            YMax = FovVertical/2f;
        }
    }
    [SerializeField, HideInInspector]
    public State OriginalState { get; private set; }
    [SerializeField, HideInInspector]
    public State CurrentState { get; private set; }
    //The X angle from the original rotation to the current rotation
    public float XBias { get; private set; }
    //The Y angle from the original rotation to the current rotation
    public float YBias { get; private set; }

    [Range(0,10)] public float ZoomSpeed = 1;
    [Range(0,10)] public float PanSeedX = 1;
    [Range(0,10)] public float PanSeedY = 1;
    public bool IsZooming { get; private set; }
    public bool IsPanning { get; private set; }
    private float _zoomDelta;
    private float _panDeltaX;
    private float _panDeltaY;
    [SerializeField] private Camera _camera;

    void OnValidate() {
        if (!_camera) _camera = Camera.main;
        if (OriginalState == null) OriginalState = new State(_camera);
        if (CurrentState == null) CurrentState = new State(_camera);
    }
    void Start() {
        OriginalState.Set();
    }
    void Update()
    {
        //Pan
        IsPanning = Input.GetButton("Fire3");
        if (IsPanning) {
            _panDeltaX = -Input.GetAxis("Mouse X") * PanSeedX * Time.deltaTime * 1000;
            _panDeltaY = Input.GetAxis("Mouse Y") * PanSeedY * Time.deltaTime * 1000;
            PanX(_panDeltaX);
            PanY(_panDeltaY);
        } else {
            _panDeltaX = _panDeltaY = 0;
        }
        //Zoom
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        IsZooming = zoomInput != 0;
        if (IsZooming) {
            _zoomDelta = -zoomInput * ZoomSpeed * Time.deltaTime * 1000;
            Zoom(_zoomDelta);
        }
        CurrentState.Set();
    }

    //delta is in degree. delta > 0 - zoom out. delta < 0 - zoom in
    internal void Zoom(float delta) {
        if (delta > 0) {
            //Cannot zoom out?
            if (CurrentState.FovVertical >= OriginalState.FovVertical) return;
            //Is touching left border?
            if (CurrentState.XMin <= OriginalState.XMin) {
                PanX( Camera.VerticalToHorizontalFieldOfView(delta, _camera.aspect)/2 );
            //Is touching right border?
            } else if (CurrentState.XMax >= OriginalState.XMax) {
                PanX( -Camera.VerticalToHorizontalFieldOfView(delta, _camera.aspect)/2 );
            }
            //Is touching top border?
            if (CurrentState.YMin <= OriginalState.YMin) {
                PanY( delta/2 );
            //Is touching bottom border?
            } else if (CurrentState.YMax >= OriginalState.YMax) {
                PanX( -delta/2 );
            }
        }
        float newFov = _camera.fieldOfView + delta;
        _camera.fieldOfView = Mathf.Clamp(newFov, 10, OriginalState.FovVertical);
    }
    //delta is in degree. delta > 0 - pan right. delta < 0 - pan left
    internal void PanX(float delta) {
        var clampedDelta = Mathf.Clamp(delta, OriginalState.XMin - CurrentState.XMin, OriginalState.XMax - CurrentState.XMax);
        _camera.transform.Rotate(transform.up, clampedDelta);
        XBias += clampedDelta;
    }
    //delta is in degree. delta > 0 - pan down. delta < 0 - pan up
    internal void PanY(float delta) {
        var clampedDelta = Mathf.Clamp(delta, OriginalState.YMin - CurrentState.YMin, OriginalState.YMax - CurrentState.YMax);
        _camera.transform.Rotate(transform.right, clampedDelta);
        YBias += clampedDelta;
    }
}
