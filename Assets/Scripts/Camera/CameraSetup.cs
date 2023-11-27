using System;
using DefaultNamespace;
using UnityEngine;

namespace CameraNameSpace
{
    public class CameraSetup : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            MoveCameraToMiddleOfGrid();
            ZoomFieldOfViewByCellSize();
        }
        
        private void MoveCameraToMiddleOfGrid()
        {
            float middleValueX = (float)GameManager.Instance.GameDataSo.WidthCellNumber / 2;
            float middleValueY = (float)GameManager.Instance.GameDataSo.HeightCellNumber / 2;
            Vector3 newCameraPosition = new Vector3(middleValueX, middleValueY, transform.position.z);
            transform.position = newCameraPosition;
        }

        private void ZoomFieldOfViewByCellSize()
        {
            
        }
    }
}