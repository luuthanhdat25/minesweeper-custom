using Grid;
using UI;
using UnityEngine;

namespace Manager
{
    public class GameDataSO : ScriptableObject
    {
        public int WidthCellNumber;
        public int HeightCellNumber;
        public int BoomNumber;
        public TileThemeSO TileThemeSo;
        public FaceThemeSO FaceThemeSo;
        public TimerThemeSO TimerThemeSo;
    }
}