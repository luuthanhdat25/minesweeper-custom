using System;
using RepeatUtil.DesignPattern.SingletonPattern;
using UnityEngine;

namespace DefaultNamespace
{
    public class InputManager : SingletonDestroyOnLoad<InputManager>
    {
        private Camera _camera;
        private bool _isLeftMouseDown;
        private bool _isRightMouseDown;
        private float _mouseScrollWheelY;

        public bool IsLeftMouseDown => _isLeftMouseDown;
        public bool IsRightMouseDown => _isRightMouseDown;
        public float MouseScollWheelY => _mouseScrollWheelY;
        
        public event EventHandler OnPauseAction;
        
        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
        }
        
        private void Update() => UpdateInputStatus();

        private void UpdateInputStatus()
        {
            _isLeftMouseDown = Input.GetMouseButtonDown(0);
            _isRightMouseDown = Input.GetMouseButtonDown(1);
            _mouseScrollWheelY = Input.GetAxis("Mouse ScrollWheel");
            if(Input.GetKeyDown(KeyCode.Escape)) 
                OnPauseAction?.Invoke(this, EventArgs.Empty);
        }
        
        public Vector3 GetMousePositionInWorld() => _camera.ScreenToWorldPoint(GetMousePositionInScreen());

        public Vector3 GetMousePositionInScreen() => Input.mousePosition;
    }
}