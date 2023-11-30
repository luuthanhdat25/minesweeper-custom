using DefaultNamespace;
using UnityEngine;


namespace CameraNameSpace
{
    public class CameraZoom : RepeatMonoBehaviour
    {
        [SerializeField] private float _lerpZoomInValue = 15f;
        [SerializeField] private float _lerpFlowMouseSpeedIn = 15f;
        [SerializeField] private float _lerpFlowMouseSpeedOut = 25f;
        [SerializeField] private CameraController _cameraController;
        
        private bool _isZoomInState = false;
        private Vector3 _focusPosition;
        private Vector3 _focusPositionInScreen;

        private byte MIN_ZOOM_IN_SIZE_CAMERA = 2;
        private byte FOCUS_RANGE = 10;
        
        protected override void LoadComponents()
        {
            base.LoadComponents();
            if (_cameraController != null) return;
            _cameraController = transform.parent.GetComponent<CameraController>();
        }

        private void LateUpdate() => HandleZoom();

        private void HandleZoom()
        {
            if (GameManager.Instance.IsPauseGame()) return;
            float mouseScrollYAxis = InputManager.Instance.MouseScollWheelY;
            if (Mathf.Approximately(mouseScrollYAxis, 0)) return;
            
            float currentOrthographicSize = _cameraController.GetCameraOrthographicSize();
            float newOrthographicSize = GetNewOrthographicSizeWithMouseScroll(mouseScrollYAxis, currentOrthographicSize);
            bool isZoomIn = newOrthographicSize < currentOrthographicSize;
            
            MoveCameraToTargetZoomPosition(isZoomIn, newOrthographicSize);
            SetNewCameraOrthographicSize(newOrthographicSize);
        }
        
        private float GetNewOrthographicSizeWithMouseScroll(float mouseScrollYAxis, float currentOrthographicSize)
        {
            float targetZoom = currentOrthographicSize - mouseScrollYAxis*_lerpZoomInValue;
            targetZoom = Mathf.Clamp(targetZoom, MIN_ZOOM_IN_SIZE_CAMERA, _cameraController.MaxSizeCamera);
            return Mathf.Lerp(
                currentOrthographicSize,
                targetZoom,
                _lerpZoomInValue * Time.deltaTime);
        }

        private void MoveCameraToTargetZoomPosition(bool isZoomIn, float newOrthographicSize)
        {
            if (isZoomIn) {
                UpdateFocusPosition();
                CameraMove(_focusPosition, _lerpFlowMouseSpeedIn);
            }
            else {
                _isZoomInState = false;
                if(Mathf.Approximately(newOrthographicSize, _cameraController.MaxSizeCamera))
                    CameraMove(_cameraController.StartCameraPosition, _lerpFlowMouseSpeedOut);
            }
        }

        private void UpdateFocusPosition()
        {
            if (!_isZoomInState) {
                _isZoomInState = true;
                _focusPosition = InputManager.Instance.GetMousePositionInWorld();
                _focusPositionInScreen = InputManager.Instance.GetMousePositionInScreen();
            }

            if (IsMousePositionOutFocusRange()) {
                _focusPosition = InputManager.Instance.GetMousePositionInWorld();
                _focusPositionInScreen = InputManager.Instance.GetMousePositionInScreen();
            }
        }

        private bool IsMousePositionOutFocusRange()
        {
            return Vector3.Distance(InputManager.Instance.GetMousePositionInScreen(), _focusPositionInScreen) 
                   > FOCUS_RANGE;
        }

        private void CameraMove(Vector3 endPoint, float lerpSpeed)
        {
            transform.parent.position 
                = Vector3.Lerp(
                    transform.parent.position, 
                    endPoint, 
                    lerpSpeed * Time.deltaTime
                    );
        }
        
        private void SetNewCameraOrthographicSize(float newOrthographicSize) 
            => _cameraController.SetCameraOrthographicSize(newOrthographicSize);
    }
}