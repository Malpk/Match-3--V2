// Copyright (C) 2017-2018 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace GameVanilla.Game.Common
{
    /// <summary>
    /// The base class used for the tiles in the visual editor.
    /// </summary>
    public class LevelTile
    {
        public Vector2Int Position;
        public ElementType elementType;

        public virtual bool TryGetTile(TilePool tilePool, out GameObject tile)
        {
            tile = null;
            return false;
        }
    }

    /// <summary>
    /// The class used for candy tiles.
    /// </summary>
    public class CandyTile : LevelTile
    {
        public CandyType type;

        public override bool TryGetTile(TilePool tilePool, out GameObject tile)
        {
            tile = tilePool.GetCandyPool((CandyColor)((int)type)).GetObject();
            return tile;
        }
    }

    /// <summary>
    /// The class used for special candy tiles.
    /// </summary>
    public class SpecialCandyTile : LevelTile
    {
        public SpecialCandyType type;

        public override bool TryGetTile(TilePool tillePool, out GameObject tile)
        {
            tile = tillePool.CreateSpecialCandyTile(this);
            return tile;
        }
    }

    /// <summary>
    /// The class used for special block tiles.
    /// </summary>
    public class SpecialBlockTile : LevelTile
    {
        public SpecialBlockType type;

        public override bool TryGetTile(TilePool tilePool, out GameObject tile)
        {
            tile = tilePool.GetSpecialBlockPool(type).GetObject();
            return tile;
        }

    }

    /// <summary>
    /// The class used for collectable tiles.
    /// </summary>
    public class CollectableTile : LevelTile
    {
        public CollectableType type;

        public override bool TryGetTile(TilePool tilePool, out GameObject tile)
        {
            tile = tilePool.GetCollectablePool(type).GetObject();
            return tile;
        }

    }

    /// <summary>
    /// The class used for hole tiles.
    /// </summary>
    public class HoleTile : LevelTile
    {
    }
}