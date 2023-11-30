using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

namespace Grid.Tilemap
{
    public class MyGrid_2 : RepeatMonoBehaviour
    {
        private UnityEngine.Tilemaps.Tilemap _tilemap;
        private int _widthCellNumber, _heightCellNumber;
        private Dictionary<byte, TileSO> _numberCellDictionary = new Dictionary<byte, TileSO>();
        private GameDataSO _gameDataSo;
        private byte[,] _valueMatrix;
        private int[] _boomIndexArray;
        private bool firstSafe = false;
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

        #region Instance Matrix
            protected override void LoadComponents()
            {
                base.LoadComponents();
                if (_tilemap != null) return;
                _tilemap = transform.GetComponentInChildren<UnityEngine.Tilemaps.Tilemap>();
            }

            private void Start()
            {
                _gameDataSo = GameManager.Instance.GameDataSo;
                _gameDataSo.TileThemeSo.Awake();
                _widthCellNumber = _gameDataSo.WidthCellNumber;
                _heightCellNumber = _gameDataSo.HeightCellNumber;
                InstantiateNumberCellDictionary();
                RandomBoomAndCalculateCellValue();
                InstantiateCells();
            }

            private void InstantiateNumberCellDictionary()
            {
                TileThemeSO tileThemeSo = _gameDataSo.TileThemeSo;
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
                TileBase[] tiles = Enumerable
                    .Repeat(
                        _gameDataSo.TileThemeSo.GetUpCellTileBaseData(), 
                        _widthCellNumber * _heightCellNumber
                        ).ToArray();
                BoundsInt boundBox 
                    = new BoundsInt(0, 0, 0, _widthCellNumber, _heightCellNumber, 1);
                _tilemap.SetTilesBlock(boundBox, tiles);
            }
        #endregion

        #region Update Tile With FloodFill
            public void UpdateTileAtWorldPosition(Vector3 worldPosition)
            {
                Vector3Int tilePosition = GetCellPositionByWorldPosition(worldPosition);
                if (!IsTilePositionInRange(tilePosition)) return;
                UpdateTile(tilePosition);
            }
            
            public Vector3Int GetCellPositionByWorldPosition(Vector3 worldPosition)
                => this._tilemap.WorldToCell(worldPosition);
            
            private bool IsTilePositionInRange(Vector3Int tilePosition)
            {
                return tilePosition.x >= 0 && tilePosition.x < _widthCellNumber &&
                       tilePosition.y >= 0 && tilePosition.y < _heightCellNumber;
            }
            
            private void UpdateTile(Vector3Int tilePosition)
            {
                if (!IsUpTile(tilePosition)) return;

                if (!IsBoom(tilePosition.x, tilePosition.y)) {
                    firstSafe = true;
                    FloodFill(tilePosition);
                }
                else if (!firstSafe) {
                    firstSafe = true;
                    FindSafeCellAndSwap(tilePosition);
                }
                else {
                    ActivateAllBoom(tilePosition);
                }
                
            }
            
            private bool IsUpTile(Vector3Int tilePosition) 
                => _tilemap.GetTile(tilePosition) == _gameDataSo.TileThemeSo.GetUpCellTileBaseData();
            
            private void FloodFill(Vector3Int tilePosition)
            {
                if (!IsDownTile(GetNumberBoomAroundTile(tilePosition)))
                {
                    _tilemap.SetTile(tilePosition, GetNumberBoomAroundTile(tilePosition));
                    return;
                }
                
                HashSet<int> cellVisited = new HashSet<int>();
                Queue<Vector3Int> cellQueue = new Queue<Vector3Int>();
                cellVisited.Add(GetIndexIntFromVector3Int(tilePosition));
                cellQueue.Enqueue(tilePosition);
                
                while (cellQueue.Count != 0)
                {
                    Vector3Int currentVertex = cellQueue.Dequeue();
                    TileSO currentTile = GetNumberBoomAroundTile(currentVertex);
                    _tilemap.SetTile(currentVertex, currentTile);
                    
                    if (IsDownTile(currentTile)) EnqueueAdjacentTiles(currentVertex, cellVisited, cellQueue);
                }
            }
            
            private bool IsDownTile(TileSO currentTile) => currentTile == _gameDataSo.TileThemeSo.GetDownCellTileBaseData();

            private void EnqueueAdjacentTiles(
                Vector3Int currentVertex, 
                HashSet<int> cellVisited,
                Queue<Vector3Int> cellQueue)
            {
                foreach (Vector3Int direction in directionVisites) {
                    Vector3Int adjacentTile = currentVertex + direction;
                    if (IsTilePositionInRange(adjacentTile)
                        && cellVisited.Add(GetIndexIntFromVector3Int(adjacentTile))) 
                    {
                        cellQueue.Enqueue(adjacentTile);
                    }
                }
            }

            private int GetIndexIntFromVector3Int(Vector3Int vector3Int) 
                => vector3Int.x * _widthCellNumber + vector3Int.y;
            
            private TileSO GetNumberBoomAroundTile(Vector3Int tilePosition) 
                => _numberCellDictionary[_valueMatrix[tilePosition.x, tilePosition.y]];
            
            private void FindSafeCellAndSwap(Vector3Int tilePosition)
            {
                for (int y = 0; y < _heightCellNumber; y++) {
                    for (int x = 0; x < _widthCellNumber; x++) {
                        if (!IsBoom(x, y))
                        {
                            //Switch position to none boom cell
                            //Add boom add plus value to around cell
                            _valueMatrix[x, y] = BOOM_VALUE;
                            for (int t = x - 1; t <= x + 1; t++) {
                                if (t < 0 || t >= _widthCellNumber) continue;
                                for (int z = y - 1; z <= y + 1; z++)
                                {
                                    if (z < 0 || z >= _heightCellNumber) continue;
                                    if (t == x && z == y) continue;
                                    if (_valueMatrix[t, z] != BOOM_VALUE)
                                        _valueMatrix[t, z]++;
                                }
                            }

                            //Remove boom and deduct value to around cell
                            _valueMatrix[tilePosition.x, tilePosition.y] = 0;
                            for (int t = tilePosition.x - 1; t <= tilePosition.x + 1; t++) {
                                if (t < 0 || t >= _widthCellNumber) continue;
                                for (int z = tilePosition.y - 1; z <= tilePosition.y + 1; z++) {
                                    if (z < 0 || z >= _heightCellNumber) continue;
                                    if (t == tilePosition.x && z == tilePosition.y) continue;
                                    if (_valueMatrix[t, z] == BOOM_VALUE)
                                        _valueMatrix[tilePosition.x, tilePosition.y]++;
                                    else
                                        _valueMatrix[t, z]--;
                                }
                            }

                            FloodFill(tilePosition);
                            Debug.Log("Safe");
                            return;
                        }
                    }
                }
            }
        #endregion
        
        #region Update Flage Cell
            public void ToogleFlagCell(Vector3 worldPosition)
            {
                Vector3Int tilePosition = GetCellPositionByWorldPosition(worldPosition);
                if (!IsTilePositionInRange(tilePosition)) return;
                
                if (IsUpTile(tilePosition)) {
                    _tilemap.SetTile(tilePosition, _gameDataSo.TileThemeSo.GetFlagCellTileBaseData());
                }else if (IsFlagTile(tilePosition)){
                    _tilemap.SetTile(tilePosition, _gameDataSo.TileThemeSo.GetUpCellTileBaseData());
                }
            }

            private bool IsFlagTile(Vector3Int tilePosition)
                => _tilemap.GetTile(tilePosition) == _gameDataSo.TileThemeSo.GetFlagCellTileBaseData();

            private void ActivateAllBoom(Vector3Int tilePosition)
            {
                Vector3Int vectorTemp = tilePosition;
                foreach (int index in _boomIndexArray)
                {
                    int x = index % _widthCellNumber;
                    int y = (index - x) / _widthCellNumber;
                    vectorTemp.Set(x, y, tilePosition.z);
                    _tilemap.SetTile(vectorTemp, _gameDataSo.TileThemeSo.GetBoomDefaultCellTileBaseData());
                }
                
                _tilemap.SetTile(tilePosition, _gameDataSo.TileThemeSo.GetBoomDeadCellTileBaseData());
                GameManager.Instance.GameOver();
                Debug.Log("Loss Game");
            }
        #endregion
    }
}
