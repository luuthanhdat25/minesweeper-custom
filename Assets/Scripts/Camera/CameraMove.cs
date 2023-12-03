using System;
using DefaultNamespace;
using Manager;
using UnityEngine;

namespace CameraNameSpace
{
    public class CameraMove : RepeatMonoBehaviour
    {
        [SerializeField] private float _panSpeed;
        [SerializeField] private int _paddingPersent = 2;
        
        [SerializeField] private CameraController _cameraController;

        private float _paddingPixel;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            if (_cameraController != null) return;
            this._cameraController = transform.parent.GetComponent<CameraController>();
        }

        private void Start()
        {
            int minLengthScreen = Math.Min(Screen.width, Screen.height);
            _paddingPixel = (float)(_paddingPersent * minLengthScreen) / 100;
        }

        private void LateUpdate() => HandleMove();

        private void HandleMove()
        {
            if (GameManager.Instance.IsPauseGame()) return;
            Vector3 mouseInScreenPosition = InputManager.Instance.GetMousePositionInScreen();
            if (!IsMouseOutsideScreenPosition(mouseInScreenPosition)) return;
            
            Vector3 moveVector = Vector3.zero;
            if (mouseInScreenPosition.y > Screen.height - _paddingPixel)
                moveVector += Vector3.up;
            else if (mouseInScreenPosition.y < _paddingPixel)
                moveVector += Vector3.down;

            if (mouseInScreenPosition.x > Screen.width - _paddingPixel)
                moveVector += Vector3.right;
            else if (mouseInScreenPosition.x < _paddingPixel)
                moveVector += Vector3.left;

            moveVector *= _panSpeed * Time.deltaTime;
            if (_cameraController.IsCameraOverPosition(moveVector)) return;
            transform.parent.position += moveVector;
        }

        private bool IsMouseOutsideScreenPosition(Vector3 mouseInScreenPosition)
        {
            return mouseInScreenPosition.y > Screen.height - _paddingPixel
                   || mouseInScreenPosition.y < _paddingPixel
                   || mouseInScreenPosition.x > Screen.width - _paddingPixel
                   || mouseInScreenPosition.x < _paddingPixel;
        }
    }
}