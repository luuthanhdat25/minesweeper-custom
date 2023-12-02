using DefaultNamespace;
using Grid;
using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private MyGrid _myGrid;
        
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