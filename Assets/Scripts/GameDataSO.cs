using Grid.Tilemap;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameDataSO : ScriptableObject
    {
        public int WidthCellNumber;
        public int HeightCellNumber;
        public int BoomNumber;
        public TileThemeSO TileThemeSo;
    }
}