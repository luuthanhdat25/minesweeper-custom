using Grid.Tilemap;
using UnityEngine;

namespace DefaultNamespace.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private MyGrid _myGrid;
        
        private void LateUpdate() => HandlePlayerAction();

        private void HandlePlayerAction()
        {
            if (InputManager.Instance.IsLeftMouseDown)
            {
                Vector3 mouseWorldPos = InputManager.Instance.GetMousePositionInWorld();
                _myGrid.DoLogicAtWorldPosition(mouseWorldPos);
            }
            
            if (InputManager.Instance.IsRightMouseDown)
            {
                Vector3 mouseWorldPos = InputManager.Instance.GetMousePositionInWorld();
                _myGrid.ToogleFlagCell(mouseWorldPos);
            }
        }
    }
}