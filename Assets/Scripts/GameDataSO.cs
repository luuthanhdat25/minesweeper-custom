using Grid.Tilemap;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 0)]
    public class GameDataSO : ScriptableObject
    {
        public int cellNumber;
        public int boomNumber;
        public TileSO upCellTile;
        public TileSO downCellTile;
        public TileSO boomDeadCellTile;
    }
}