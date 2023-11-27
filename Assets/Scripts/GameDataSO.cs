using Grid.Tilemap;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameDataSO : ScriptableObject
    {
        public int WidthCellNumber;
        public int HeightCellNumber;
        public int BoomNumber;
        public TileSO UpCellTile;
        public TileSO DownCellTile;
        public TileSO FlagCellTile;
        public TileSO BoomDeadCellTile;
        public TileSO BoomDefaultCellTile;
        public TileSO OneCellTile;
        public TileSO TwoCellTile;
        public TileSO ThreeCellTile;
        public TileSO FourCellTile;
        public TileSO FiveCellTile;
        public TileSO SixCellTile;
        public TileSO SevenCellTile;
        public TileSO EightCellTile;

    }
}