using System;
using DefaultNamespace;
using UnityEngine;

namespace CameraNameSpace
{
    public class CameraController : RepeatMonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private float _maxSizeCamera;
        private Vector3 _startCameraPosition;
        
        public float MaxSizeCamera => _maxSizeCamera;
        public Vector3 StartCameraPosition => _startCameraPosition;
        
        protected override void Awake()
        {
            base.Awake();
            if (_camera != null) return;
            this._camera = GetComponent<Camera>();
        }
        
        private void Start()
        {
            GameDataSO gameDataSo = GameManager.Instance.GameDataSo;
            _maxSizeCamera = Math.Max(gameDataSo.HeightCellNumber, gameDataSo.WidthCellNumber) * 3f/ 4f;
            MoveCameraToMiddleOfGrid();
            SetCameraOrthographicSize(_maxSizeCamera);
        }
        
        //Setup the camera position
        private void MoveCameraToMiddleOfGrid()
        {
            float middleValueX = (float)GameManager.Instance.GameDataSo.WidthCellNumber / 2;
            float middleValueY = (float)GameManager.Instance.GameDataSo.HeightCellNumber / 2;
            _startCameraPosition = new Vector3(middleValueX, middleValueY, transform.position.z);
            transform.position = _startCameraPosition;
        }

        public void SetCameraOrthographicSize(float size) => _camera.orthographicSize = size;
        
        public float GetCameraOrthographicSize() => _camera.orthographicSize;
        
        public bool IsCameraOverPosition(Vector3 moveVector) 
            => Vector3.Distance(transform.position + moveVector, _startCameraPosition) > _maxSizeCamera/2;
    }
}