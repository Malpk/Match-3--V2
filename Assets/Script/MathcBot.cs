using System.Collections.Generic;
using GameVanilla.Game.Common;
using System.Collections;
using UnityEngine;

public class MathcBot : MonoBehaviour
{
    [SerializeField] private Texture2D _masks;
    [SerializeField] private GameBoard _tileHolder;
    [SerializeField] private List<Candy> _tiles = new List<Candy>();
    [SerializeField] private List<List<Candy>> _temp = new List<List<Candy>>();

    private Candy[,] _map;
    private List<int[,]> _temps = new List<int[,]>();

    private int _count;

    private void Awake()
    {

    }

    public void Play()
    {
        _tiles.Clear();
        StopAllCoroutines();
        StartCoroutine(Playing());
    }

    private IEnumerator Playing()
    {
        yield return null;
        _temp.Clear();
        yield return Maping();
        var size = 4;
        var list = new List<BotSwapeItem>();
        for (int y = 0; y < _tileHolder.Height - size; y++)
        {
            for (int x = 0; x < _tileHolder.Width - size; x++)
            {
                foreach (var item in SearchHorizontal(_map[y, x], size))
                {
                    list.Add(item);
                }
                foreach (var item in SearchVertical(_map[y, x], size))
                {
                    list.Add(item);
                }
            }
            yield return null;
        }
        if (list.Count > 0)
        {
            var swape = list[Random.Range(0, list.Count)];
            _tileHolder.InputBoard(swape.Tile, swape.Select);
            Debug.Log($"candy {swape.Select.color} => {swape.Tile.color} = {swape.CountTemp}");
            
        }
        
        yield return null;
    }

    #region searchTemp
    private List<BotSwapeItem> SearchHorizontal(Candy candy, int size)
    {
        var list = new List<BotSwapeItem>();
        var map = CopyMap(candy, size);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                var temp = map[y, x];
                map[y, x] = map[y, x + 1];
                map[y, x + 1] = temp;
                var countTemp = CheakHorizontalTemp(map) + CheakVeritacalTemp(map);
                map[y, x + 1] = map[y, x];
                map[y, x] = temp;
                if (countTemp > 0)
                {
                    var item = new BotSwapeItem();
                    item.Select = _map[candy.y + y, candy.x + x];
                    item.Tile = _map[candy.y + y, candy.x + x + 1];
                    item.CountTemp = countTemp;
                    list.Add(item);
                }
            }
        }
        return list;
    }

    private List<BotSwapeItem> SearchVertical(Candy candy, int size)
    {
        var list = new List<BotSwapeItem>();
        var map = CopyMap(candy, size);
        for (int y = 0; y < size - 1; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var temp = map[y, x];
                map[y, x] = map[y + 1, x];
                map[y + 1, x] = temp;
                var countTemp = CheakHorizontalTemp(map)
                    + CheakVeritacalTemp(map);
                temp = map[y, x];
                map[y, x] = map[y + 1, x];
                map[y + 1, x] = temp;
                if (countTemp > 0)
                {
                    var item = new BotSwapeItem();
                    item.Select = _map[candy.y + y, candy.x + x];
                    item.Tile = _map[candy.y + y + 1, candy.x + x];
                    item.CountTemp = countTemp;
                    list.Add(item);
                }
            }
        }
        return list;
    }



    public int CheakHorizontalTemp(CandyColor[,] map)
    {
        var countTemp = 0;
        for (int y = 0; y < map.GetLength(0); y++)
        { 
            var count = 0;
            var color = CandyColor.None;
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (color == CandyColor.None)
                {
                    color = map[y, x];
                    count = 1;
                }
                else if (color == map[y, x])
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        countTemp++;
                    count = 1;
                    color = map[y, x];
                }
            }
        }
        return countTemp;
    }

    public int CheakVeritacalTemp(CandyColor[,] map)
    {
        var countTemp = 0;
        for (int x = 0; x < map.GetLength(1); x++)
        {
            var count = 0;
            var color = CandyColor.None;
            for (int y = 0; y < map.GetLength(0); y++)
            {
                if (color == CandyColor.None)
                {
                    color = map[y, x];
                    count = 1;
                }
                else if (color == map[y, x])
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        countTemp++;
                    count = 1;
                    color = map[y, x];
                }
            }
        }
        return countTemp;
    }

    #endregion

    public CandyColor[,] CopyMap(Candy center, int size)
    {
        var map = new CandyColor[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                map[y, x] = _map[center.y + y, center.x + x].color;
            }
        }
        return map;
    }

    private IEnumerator Maping()
    {
        _map = new Candy[_tileHolder.Height, _tileHolder.Width];
        _tiles.Clear();
        for (int y = 0; y < _tileHolder.Height; y++)
        {
            for (int x = 0; x < _tileHolder.Width; x++)
            {
                var tile = _tileHolder.GetTile(x, y);
                if (tile)
                {
                    _map[y, x] = tile.GetComponent<Candy>();
                    _tiles.Add(_map[y, x]);
                }
                else
                {
                    _map[y, x] = null;
                }
                yield return null;
            }
        }
    }

}
