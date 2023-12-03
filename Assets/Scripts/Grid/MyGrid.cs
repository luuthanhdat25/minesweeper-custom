using System.Collections.Generic;
using Manager;
using RepeatUtil.DesignPattern.SingletonPattern;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grid
{
    public partial class MyGrid : SingletonDestroyOnLoad<MyGrid>
    {
        private Tilemap _tilemap;
        private int _widthCellNumber, _heightCellNumber, _nonBoomedCellNumber;
        private Dictionary<byte, TileSO> _numberCellDictionary;
        private GameDataSO _gameDataSo;
        private byte[,] _valueMatrix;
        private int[] _boomIndexArray;
        //Constants
        private const byte DOWN_VALUE = 0;
        private const byte UP_VALUE = 10;
        private const byte FLAG_VALUE = 20;
        private const byte BOOM_VALUE = 30;
        private static readonly Vector3Int[] directionVisites 
            = {
                Vector3Int.right, 
                Vector3Int.down, 
                Vector3Int.left, 
                Vector3Int.up
            };

        protected override void LoadComponents()
        {
            base.LoadComponents();
            if (_tilemap != null) return;
            _tilemap = transform.GetComponentInChildren<Tilemap>();
        }

        private void Start()
        {
            _gameDataSo = GameManager.Instance.GameDataSo;
            _gameDataSo.TileThemeSo.Awake();
            _widthCellNumber = _gameDataSo.WidthCellNumber;
            _heightCellNumber = _gameDataSo.HeightCellNumber;
            _nonBoomedCellNumber = (_widthCellNumber * _heightCellNumber) - _gameDataSo.BoomNumber; 
            
            InstantiateNumberCellDictionary();
            RandomBoomAndCalculateCellValue();
            InstantiateCells();
        }

        private void InstantiateNumberCellDictionary()
        {
            var tileThemeSo = _gameDataSo.TileThemeSo;
            _numberCellDictionary = new Dictionary<byte, TileSO>()
            {
                { 0, tileThemeSo.GetDownCellTileBaseData() },
                { 1, tileThemeSo.GetOneCellTileBaseData() },
                { 2, tileThemeSo.GetTwoCellTileBaseData() },
                { 3, tileThemeSo.GetThreeCellTileBaseData() },
                { 4, tileThemeSo.GetFourCellTileBaseData() },
                { 5, tileThemeSo.GetFiveCellTileBaseData() },
                { 6, tileThemeSo.GetSixCellTileBaseData() },
                { 7, tileThemeSo.GetSevenCellTileBaseData() },
                { 8, tileThemeSo.GetEightCellTileBaseData() }
            };
        }

        private void RandomBoomAndCalculateCellValue()
        {
            int arraySize = _widthCellNumber * _heightCellNumber;
            int boomNumber = _gameDataSo.BoomNumber; 
            byte[] boomIndexBools = new byte[arraySize];
            _boomIndexArray = new int[boomNumber];
            _valueMatrix = new byte[_widthCellNumber, _heightCellNumber];
            
            // Fisher-Yates Shuffle
            for (int i = boomNumber, j = arraySize - 1; i > 0; i--, j--)
                boomIndexBools[j] = BOOM_VALUE;

            System.Random random = new System.Random();
            for (int i = arraySize - 1; i > 0; i--) {
                int randomNumber = random.Next(0, i + 1);
                if(randomNumber != i){
                    (boomIndexBools[i], boomIndexBools[randomNumber]) 
                    = (boomIndexBools[randomNumber], boomIndexBools[i]);
                }
            }
            
            //Calculate cell values of matrix
            int boomIndex = 0;
            for (int i = 0; i < arraySize; i++)
            {
                if (boomNumber <= 0) break;
                if (boomIndexBools[i] != BOOM_VALUE) continue;
                _boomIndexArray[boomIndex++] = i;
                int x = i % _widthCellNumber;
                int y = (i - x) / _widthCellNumber;
                _valueMatrix[x, y] = BOOM_VALUE;

                for (int t = x - 1; t <= x + 1; t++) {
                    if (t < 0 || t >= _widthCellNumber) continue;
                    for (int z = y - 1; z <= y + 1; z++) {
                        if (z < 0 || z >= _heightCellNumber) continue;
                        if (t == x && z == y) continue;
                        if (_valueMatrix[t, z] != BOOM_VALUE)
                            _valueMatrix[t, z]++;
                    }
                }
                boomNumber--;
            }
        }

        private bool IsBoom(int x, int y) => _valueMatrix[x,y] >= BOOM_VALUE;

        private void InstantiateCells()
        {
            TileBase[] tiles = new TileBase[_widthCellNumber * _heightCellNumber];
            TileBase upTile = _gameDataSo.TileThemeSo.GetUpCellTileBaseData();

            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = upTile;

            BoundsInt boundBox 
                = new BoundsInt(0, 0, 0, _widthCellNumber, _heightCellNumber, 1);
            _tilemap.SetTilesBlock(boundBox, tiles);
        }
    }
}
