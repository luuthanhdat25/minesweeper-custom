using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grid.Tilemap
{
    [CreateAssetMenu(fileName = "New Tile", menuName = "New Tile")]
    public class TileSO : TileBase
    {
        [SerializeField] private Sprite sprite;
        //[SerializeField] private TileType tileType;
        
        public Sprite GetSprite => sprite;
        //public TileType TileType => tileType;
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = this.sprite;
        }
    }
}