using Grid.Tilemap;
using UnityEngine;

namespace DefaultNamespace.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private MyGrid_2 _myGrid;
        
        private void LateUpdate() => HandlePlayerAction();

        private void HandlePlayerAction()
        {
            if (InputManager.Instance.IsLeftMouseDown)
            {
                Vector3 mouseWorldPos = InputManager.Instance.GetMousePositionInWorld();
                _myGrid.UpdateTileAtWorldPosition(mouseWorldPos);
            }
            
            if (InputManager.Instance.IsRightMouseDown)
            {
                Vector3 mouseWorldPos = InputManager.Instance.GetMousePositionInWorld();
                _myGrid.ToogleFlagCell(mouseWorldPos);
            }
        }
    }
}