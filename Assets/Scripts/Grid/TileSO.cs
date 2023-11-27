using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Grid.Tilemap
{
    [CreateAssetMenu(fileName = "New Tile", menuName = "New Tile")]
    public class TileSO : TileBase
    {
        public Sprite Sprite;
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = this.Sprite;
        }
    }
}