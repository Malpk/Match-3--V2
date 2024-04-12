using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameVanilla.Game.Common;

public class BoardGrid : MonoBehaviour
{
    [SerializeField] private Vector2 _tileSize;
    [Header("Reference")]
    [SerializeField] private GameBoard _board;
    [SerializeField] private TilePool _tilePool;
    [SerializeField] private Transform _boardCenter;

    private GameObject[,] _tiles;
    private readonly List<CollectableType> eligibleCollectables = new List<CollectableType>();

    public List<GameObject> Tiles = new List<GameObject>();
    public List<Vector2> tilePositions = new List<Vector2>();

    private Level _level;

    private Vector2 _startPosition; 

    public void SetLevel(Level level)
    {
        _level = level;
        _startPosition = new Vector2((level.width - 1) * (_tileSize.x), 
            (level.height - 1) * (_tileSize.y));
        _startPosition /= 2f;
    }

    public void CreateBoard()
    {
        _tiles = new GameObject[_level.height, _level.width];
        SetCollecteble(_level);
        CreateBackground();
        for (var j = 0; j < _level.height; j++)
        {
            for (var i = 0; i < _level.width; i++)
            {
                var levelTile = _level.tiles[i + (j * _level.width)];
                levelTile.Position = new Vector2Int(j, i);
                var tile = CreateTileFromLevel(levelTile).GetComponent<Tile>();
                tile.GetComponent<Tile>().board = _board;
                tile.SetPosition(i, j);
                tile.transform.parent = _boardCenter;
                tile.transform.localPosition = GetPosition(i, j);
                if (tile != null)
                {
                    var collectable = tile.GetComponent<Collectable>();
                    var cidx = eligibleCollectables.FindIndex(x => x == collectable.type);
                    if (cidx != -1)
                    {
                        eligibleCollectables.RemoveAt(cidx);
                    }
                }
                _tiles[i, j] = tile.gameObject;
                Tiles.Add(tile.gameObject);
            }
        }
    }

    private void CreateBackground()
    {
        for (var j = 0; j < _level.height; j++)
        {
            for (var i = 0; i < _level.width; i++)
            {
                var levelTile = _level.tiles[i + (j * _level.width)];
                if (!(levelTile is HoleTile))
                {
                    var bgTile = _tilePool.CreateBackTile(i, j);
                    bgTile.transform.parent = _boardCenter;
                    bgTile.transform.localPosition = GetPosition(i, j);
                    tilePositions.Add(bgTile.transform.localPosition);
                }
            }
        }
    }

    private Vector2 GetPosition(int x, int y)
    {
        return new Vector2(x * _tileSize.x - _startPosition.x,
                    -y * _tileSize.y + _startPosition.y);
    }

    private void SetCollecteble(Level level)
    {
        foreach (var goal in level.goals)
        {
            if (goal is CollectCollectableGoal)
            {
                var collectableGoal = goal as CollectCollectableGoal;
                for (var i = 0; i < collectableGoal.amount; i++)
                {
                    eligibleCollectables.Add(collectableGoal.collectableType);
                }
            }
        }
    }

    #region Create


    public GameObject CreateTileFromLevel(LevelTile levelTile)
    {
        if (levelTile is CandyTile candyTile)
        {
            if (candyTile.type == CandyType.RandomCandy)
            {
                Debug.Log(levelTile);
                return CreateTile(levelTile.Position, false);
            }
            else
            {
                return _tilePool.GetCandyPool((CandyColor)((int)candyTile.type)).GetObject();
            }
        }
        else
        {
            return _tilePool.CreateTileFromLevel(levelTile);
        }
    }

    public GameObject CreateTile(int x, int y, bool runtime)
    {
       return CreateTile(new Vector2Int(x, y), runtime);
    }

    public GameObject CreateTile(Vector2Int position, bool runtime)
    {
        var eligibleTiles = new List<CandyColor>();
        eligibleTiles.AddRange(_level.availableColors);

        var leftTile1 = GetTile(position.x - 1, position.y);
        var leftTile2 = GetTile(position.x - 2, position.y);
        if (leftTile1 && leftTile2)
        {
            if(leftTile1.color == leftTile2.color)
                eligibleTiles.Remove(eligibleTiles.Find(t => t == leftTile1.color));
        }

        var topTile1 = GetTile(position.x, position.y - 1);
        var topTile2 = GetTile(position.x, position.y - 2);
        if (topTile1 && topTile2 && topTile1.color == topTile2.color)
        {
            if(topTile1.color == topTile2.color)
                eligibleTiles.Remove(eligibleTiles.Find(t => t == topTile1.color));
        }

        return CreateTile(eligibleTiles, runtime);
    }

    private GameObject CreateTile(List<CandyColor> eligibleTiles, bool mode)
    {
        if (mode && eligibleCollectables.Count > 0)
        {
            var tileChance = Random.Range(0, 100);
            if (tileChance <= _level.collectableChance)
            {
                var idx = Random.Range(0, eligibleCollectables.Count);
                var collectable = eligibleCollectables[idx];
                eligibleCollectables.RemoveAt(idx);
                return _tilePool.GetCollectablePool(collectable).GetObject();
            }
            else
            {
                return _tilePool.GetCandyPool(eligibleTiles[Random.Range(0, eligibleTiles.Count)]).GetObject();
            }
        }
        else
        {
            return _tilePool.GetCandyPool(eligibleTiles[Random.Range(0, eligibleTiles.Count)]).GetObject();
        }
    }

    #endregion

    

    public Candy GetTile(int x, int y)
    {
        if (x >= 0 && x < _level.width && y >= 0 && y < _level.height)
        {
            if(_tiles[x, y])
                return _tiles[x, y].GetComponent<Candy>();
        }
        return null;
    }

}