using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grid.Tilemap
{
    public class MyGrid: RepeatMonoBehaviour
    {
        private UnityEngine.Tilemaps.Tilemap _tilemap;
        private int _cellNumber;
        private Dictionary<int, TileSO> _numberCellDictionary;
        [SerializeField] private TileSO _upCellTile;
        [SerializeField] private TileSO _downCellTile;
        [SerializeField] private TileSO _flagCellTile;
        [SerializeField] private TileSO _boomDeadCellTile;
        [SerializeField] private TileSO _boomDefaultCellTile;
        [SerializeField] private TileSO _oneCellTile;
        [SerializeField] private TileSO _twoCellTile;
        [SerializeField] private TileSO _threeCellTile;
        [SerializeField] private TileSO _fourCellTile;
        [SerializeField] private TileSO _fiveCellTile;
        [SerializeField] private TileSO _sixCellTile;
        [SerializeField] private TileSO _sevenCellTile;
        [SerializeField] private TileSO _eightCellTile;
        private bool[,] _boomMatrix;
        private bool firstSafe = false;
        
        public UnityEngine.Tilemaps.Tilemap Tilemap => _tilemap; 
        
        protected override void LoadComponents()
        {
            base.LoadComponents();
            if (_tilemap != null) return;
            _tilemap = transform.GetComponentInChildren<UnityEngine.Tilemaps.Tilemap>();
        }

        private void Start()
        {
            _cellNumber = GameManager.Instance.GameDataSo.cellNumber;
            //_upCellTile = GameManager.Instance.GameDataSo.upCellTile;
            InstantiateNumberCellDictionary();
            InstantiateBooms();
            InstantiateCells();
        }

        private void InstantiateNumberCellDictionary()
        {
            _numberCellDictionary = new Dictionary<int, TileSO>();
            _numberCellDictionary.Add(0, _downCellTile);
            _numberCellDictionary.Add(1, _oneCellTile);
            _numberCellDictionary.Add(2, _twoCellTile);
            _numberCellDictionary.Add(3, _threeCellTile);
            _numberCellDictionary.Add(4, _fourCellTile);
            _numberCellDictionary.Add(5, _fiveCellTile);
            _numberCellDictionary.Add(6, _sixCellTile);
            _numberCellDictionary.Add(7, _sevenCellTile);
            _numberCellDictionary.Add(8, _eightCellTile);
        }

        private void InstantiateBooms()
        {
            int boomNumber = GameManager.Instance.GameDataSo.boomNumber;
            int arraySize = _cellNumber * _cellNumber;
            System.Random random = new System.Random();
            bool[] boomIndexBools = new bool[arraySize];
            int copyBoomNumber = boomNumber;
            
            for (int i = arraySize - 1; i >= 0; i--)
            {
                if (copyBoomNumber > 0) {
                    boomIndexBools[i] = true;
                    copyBoomNumber--;
                }else break;
            }
            
            for (int i = arraySize - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (boomIndexBools[i], boomIndexBools[j]) = (boomIndexBools[j], boomIndexBools[i]);
            }
            
            _boomMatrix = new bool[_cellNumber, _cellNumber];
            copyBoomNumber = boomNumber;
            for (int i = 0; i < _cellNumber * _cellNumber; i++)
            {
                if (copyBoomNumber > 0)
                {
                    if (boomIndexBools[i])
                    {
                        int y = i % _cellNumber;
                        int x = (i - y) / _cellNumber;
                        _boomMatrix[x, y] = true;
                        copyBoomNumber--;
                    }
                }else break;
            }
        }
        
        private void InstantiateCells()
        {
            TileBase[] tiles = System.Linq.Enumerable.Repeat(_upCellTile, _cellNumber*_cellNumber).ToArray();
            BoundsInt boundBox = new BoundsInt(0, 0, 0, _cellNumber, _cellNumber, 1);
            _tilemap.SetTilesBlock(boundBox, tiles);
        }

        public Vector3Int GetCellPositionByWorldPosition(Vector3 worldPosition)
            => this._tilemap.WorldToCell(worldPosition);

        public void SetTileAtTilePosition(Vector3Int tilePosition, TileSO newTile)
        {
            _tilemap.SetTile(tilePosition, newTile);
        }

        public void ToogleFlagCell(Vector3 worldPosition)
        {
            Vector3Int tilePosition = GetCellPositionByWorldPosition(worldPosition);
            if (!IsTilePositionInRange(tilePosition)) return;
            if (IsUpTile(tilePosition)) {
                _tilemap.SetTile(tilePosition, _flagCellTile);
            }else if (IsFlagTile(tilePosition)){
                _tilemap.SetTile(tilePosition, _upCellTile);
            }
        }
        
        private bool IsFlagTile(Vector3Int tilePosition) 
            => _tilemap.GetTile(tilePosition) == _flagCellTile;

        public void DoLogicAtWorldPosition(Vector3 worldPosition)
        {
            Vector3Int tilePosition = GetCellPositionByWorldPosition(worldPosition);
            if (!IsTilePositionInRange(tilePosition)) return;
            UpdateTile(tilePosition);
        }
        
        private bool IsTilePositionInRange(Vector3Int tilePosition)
        {
            return tilePosition.x >= 0 && tilePosition.x < _cellNumber &&
                   tilePosition.y >= 0 && tilePosition.y < _cellNumber;
        }

        private void UpdateTile(Vector3Int tilePosition)
        {
            if (!IsUpTile(tilePosition)) return;
            
            if (IsBoomTile(tilePosition))
            {
                if (!firstSafe)
                {
                    for (int y = 0; y < _cellNumber; y++)
                    {
                        for (int x = 0; x < _cellNumber; x++)
                        {
                            if (!_boomMatrix[x, y])
                            { 
                                _boomMatrix[x, y] = true;
                                firstSafe = true;
                                _boomMatrix[tilePosition.x, tilePosition.y] = false;
                                FloodFill(tilePosition);
                                Debug.Log("Safe");
                                return;
                            }
                        }
                    }
                }
                else _tilemap.SetTile(tilePosition, _boomDeadCellTile);
                //End Game
            }
            else
            {
                firstSafe = true;
                FloodFill(tilePosition);
            }
        }

        //Hashset
        private void FloodFill(Vector3Int tilePosition)
        {
            HashSet<int> cellVisited = new HashSet<int>();
            Queue<Vector3Int> cellQueue = new Queue<Vector3Int>();
            cellVisited.Add(GetIndexIntFromVector3Int(tilePosition));
            cellQueue.Enqueue(tilePosition);
            
            while (cellQueue.Count != 0)
            {
                Vector3Int currentVertex = cellQueue.Dequeue();
                TileSO currentTile = GetNumberBoomAroundTile(currentVertex);
                _tilemap.SetTile(currentVertex, currentTile);
                if (currentTile == _downCellTile)
                {
                    // Right
                    if (currentVertex.x + 1 < _cellNumber)
                    {
                        if (cellVisited.Add(GetIndexIntFromVector3Int(currentVertex + Vector3Int.right)))
                            cellQueue.Enqueue(currentVertex + Vector3Int.right);
                    }
                    // Bottom
                    if (currentVertex.y - 1 >= 0)
                    {
                        if (cellVisited.Add(GetIndexIntFromVector3Int(currentVertex + Vector3Int.down)))
                            cellQueue.Enqueue(currentVertex + Vector3Int.down);
                    }
                    // Left
                    if (currentVertex.x - 1 >= 0)
                    {
                        if (cellVisited.Add(GetIndexIntFromVector3Int(currentVertex + Vector3Int.left)))
                            cellQueue.Enqueue(currentVertex + Vector3Int.left);
                    }
                    // Up
                    if (currentVertex.y + 1 < _cellNumber)
                    {
                        if (cellVisited.Add(GetIndexIntFromVector3Int(currentVertex + Vector3Int.up)))
                            cellQueue.Enqueue(currentVertex + Vector3Int.up);
                    }
                }
            }
        }

        private int GetIndexIntFromVector3Int(Vector3Int vector3Int) 
            => vector3Int.x *_cellNumber + vector3Int.y;

        private bool IsUpTile(Vector3Int tilePosition) 
            => _tilemap.GetTile(tilePosition) == _upCellTile;

        private bool IsBoomTile(Vector3Int tilePosition) 
            => _boomMatrix[tilePosition.x, tilePosition.y];

        private TileSO GetNumberBoomAroundTile(Vector3Int tilePosition)
        {
            int x = tilePosition.x, y = tilePosition.y;
            int countBoom = 0;
            Vector3Int travelVector3Int = new Vector3Int();
            for (int i = x - 1; i <= x + 1; i++)
            {
                if(i >= _cellNumber || i < 0) continue;
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if(j >= _cellNumber || j < 0) continue;
                    if(i == x && j == y) continue;
                    
                    travelVector3Int.Set(i, j, 0);
                    if (!IsUpTile(travelVector3Int) && !IsFlagTile(travelVector3Int))continue;
                    
                    if (IsBoomTile(i, j)) countBoom++;
                }
            }
            return _numberCellDictionary[countBoom];
        }
        
        private bool IsBoomTile(int x, int y) => _boomMatrix[x, y];
    }
}