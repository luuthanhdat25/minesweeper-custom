using System;
using DefaultNamespace;
using Grid;
using Manager;
using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private MyGrid _myGrid;

        private void Start() => _myGrid = MyGrid.Instance;

        private void LateUpdate() => HandlePlayerAction();

        private void HandlePlayerAction()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;
            
            if (InputManager.Instance.IsLeftMouseDown)
                _myGrid.UpdateTileAtWorldPosition(GetMouseWorldPosition());
            
            if (InputManager.Instance.IsRightMouseDown)
                _myGrid.ToogleFlagCell(GetMouseWorldPosition());
        }
        
        private Vector3 GetMouseWorldPosition() => InputManager.Instance.GetMousePositionInWorld();
    }
}