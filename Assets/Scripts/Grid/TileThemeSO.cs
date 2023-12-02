using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grid
{
    [CreateAssetMenu(fileName = "New Tile Theme", menuName = "Tile theme", order = 0)]
    public class TileThemeSO: ScriptableObject
    {
        [SerializeField] private Sprite UpCellTile;
        [SerializeField] private Sprite DownCellTile;
        [SerializeField] private Sprite FlagCellTile;
        [SerializeField] private Sprite BoomDeadCellTile;
        [SerializeField] private Sprite BoomDefaultCellTile;
        [SerializeField] private Sprite OneCellTile;
        [SerializeField] private Sprite TwoCellTile;
        [SerializeField] private Sprite ThreeCellTile;
        [SerializeField] private Sprite FourCellTile;
        [SerializeField] private Sprite FiveCellTile;
        [SerializeField] private Sprite SixCellTile;
        [SerializeField] private Sprite SevenCellTile;
        [SerializeField] private Sprite EightCellTile;

        private Dictionary<Sprite, TileSO> _tileBaseDataDictionary;

        public void Awake()
        {
            _tileBaseDataDictionary = new Dictionary<Sprite, TileSO>();

            AddTileBaseData(UpCellTile);
            AddTileBaseData(DownCellTile);
            AddTileBaseData(FlagCellTile);
            AddTileBaseData(BoomDeadCellTile);
            AddTileBaseData(BoomDefaultCellTile);
            AddTileBaseData(OneCellTile);
            AddTileBaseData(TwoCellTile);
            AddTileBaseData(ThreeCellTile);
            AddTileBaseData(FourCellTile);
            AddTileBaseData(FiveCellTile);
            AddTileBaseData(SixCellTile);
            AddTileBaseData(SevenCellTile);
            AddTileBaseData(EightCellTile);
        }

        private void AddTileBaseData(Sprite sprite)
        {
            if (_tileBaseDataDictionary.ContainsKey(sprite)) return;
            _tileBaseDataDictionary.Add(sprite, GetTileBaseDataBySprite(sprite));
        }
        
        private TileSO GetTileBaseDataBySprite(Sprite sprite)
        {
            TileSO tileSo = ScriptableObject.CreateInstance<TileSO>();
            tileSo.Sprite = sprite;
            return tileSo;
        }

        public TileSO GetUpCellTileBaseData() => _tileBaseDataDictionary[UpCellTile];
        public TileSO GetDownCellTileBaseData() => _tileBaseDataDictionary[DownCellTile];
        public TileSO GetFlagCellTileBaseData() => _tileBaseDataDictionary[FlagCellTile];
        public TileSO GetBoomDeadCellTileBaseData() => _tileBaseDataDictionary[BoomDeadCellTile];
        public TileSO GetBoomDefaultCellTileBaseData() => _tileBaseDataDictionary[BoomDefaultCellTile];
        public TileSO GetOneCellTileBaseData() => _tileBaseDataDictionary[OneCellTile];
        public TileSO GetTwoCellTileBaseData() => _tileBaseDataDictionary[TwoCellTile];
        public TileSO GetThreeCellTileBaseData() => _tileBaseDataDictionary[ThreeCellTile];
        public TileSO GetFourCellTileBaseData() => _tileBaseDataDictionary[FourCellTile];
        public TileSO GetFiveCellTileBaseData() => _tileBaseDataDictionary[FiveCellTile];
        public TileSO GetSixCellTileBaseData() => _tileBaseDataDictionary[SixCellTile];
        public TileSO GetSevenCellTileBaseData() => _tileBaseDataDictionary[SevenCellTile];
        public TileSO GetEightCellTileBaseData() => _tileBaseDataDictionary[EightCellTile];
    }

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