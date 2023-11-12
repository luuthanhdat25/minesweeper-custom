using System;
using DefaultNamespace;
using UnityEngine;

namespace CameraNameSpace
{
    public class CameraSetup : MonoBehaviour
    {
        private Transform _cameraTransform;
        private Camera _camera;

        private void Start()
        {
            SetUpProperty();
            MoveCameraToMiddleOfGrid();
            ZoomFieldOfView();
        }

        private void SetUpProperty()
        {
            _camera = Camera.main;
            _cameraTransform = transform.parent;
        }
        
        private void MoveCameraToMiddleOfGrid()
        {
            float middleValue = (float)GameManager.Instance.GameDataSo.cellNumber / 2;
            Vector3 newCameraPosition = new Vector3(middleValue, middleValue, _cameraTransform.position.z);
            _cameraTransform.position = newCameraPosition;
        }

        private void ZoomFieldOfView()
        {
            
        }
    }
}