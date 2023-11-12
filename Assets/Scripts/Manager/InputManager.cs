using RepeatUtil.DesignPattern.SingletonPattern;
using UnityEngine;

namespace DefaultNamespace
{
    public class InputManager : SingletonDestroyOnLoad<InputManager>
    {
        private Camera _camera;
        private bool _isLeftMouseDown;
        private bool _isRightMouseDown;

        public bool IsLeftMouseDown => _isLeftMouseDown;
        public bool IsRightMouseDown => _isRightMouseDown;
        
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
        }
        
        public Vector3 GetMousePositionInWorld()
        {
            Vector3 mousePositionInScreen = Input.mousePosition;
            return _camera.ScreenToWorldPoint(mousePositionInScreen);
        }
    }
}