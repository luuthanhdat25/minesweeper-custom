using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Grid
{
    public partial class MyGrid
    {
        public event EventHandler OnChooseTileAction;
        
        private bool firstSafe = false;
        
        #region Update Tile With FloodFill
            public void UpdateTileAtWorldPosition(Vector3 worldPosition)
            {
                Vector3Int tilePosition = GetCellPositionByWorldPosition(worldPosition);
                if (!IsTilePositionInRange(tilePosition)) return;
                UpdateTile(tilePosition);
            }
            
            public Vector3Int GetCellPositionByWorldPosition(Vector3 worldPosition)
                => _tilemap.WorldToCell(worldPosition);
            
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
                    OnChooseTileAction?.Invoke(this, EventArgs.Empty);
                }
                else if (!firstSafe) {
                    firstSafe = true;
                    FindSafeCellAndSwap(tilePosition);
                    OnChooseTileAction?.Invoke(this, EventArgs.Empty);
                }
                else {
                    ActivateAllBoom(tilePosition);
                }
            }
            
            private bool IsUpTile(Vector3Int tilePosition) 
                => _tilemap.GetTile(tilePosition) == _gameDataSo.TileThemeSo.GetUpCellTileBaseData();
            
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

                    if (IsDownTile(currentTile)) EnqueueAdjacentTiles(currentVertex, cellVisited, cellQueue);
                }
                DeductNonBoomedCell(cellVisited.Count);
            }
            
            private bool IsDownTile(TileSO currentTile) 
                => currentTile == _gameDataSo.TileThemeSo.GetDownCellTileBaseData();
           
            private void EnqueueAdjacentTiles(
                Vector3Int currentVertex, 
                HashSet<int> cellVisited,
                Queue<Vector3Int> cellQueue)
            {
                foreach (Vector3Int direction in directionVisites) {
                    Vector3Int adjacentTile = currentVertex + direction;
                    if (IsTilePositionInRange(adjacentTile)
                        && IsUpTile(adjacentTile)
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
            
            private void DeductNonBoomedCell(int cellReplace)
            {
                _nonBoomedCellNumber -= cellReplace;
                if (!IsWinGame()) return;
                GameManager.instance.WinGame();
            }

            private bool IsWinGame() => _nonBoomedCellNumber == 0;
            
            // Find the safe cell and swap if the first cell is already is the boom
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
                    
                OnChooseTileAction?.Invoke(this, EventArgs.Empty);
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
            }
        #endregion
    }
}