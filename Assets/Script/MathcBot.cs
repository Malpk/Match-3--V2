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
        _temp.Clear();
        yield return Maping();
        var size = 4;
        var list = new Dictionary<Candy, int>();
        for (int y = 0; y < _tileHolder.Height - size; y++)
        {
            for (int x = 0; x < _tileHolder.Width - size; x++)
            {
                foreach (var item in SearchHorizontal(_map[y, x], size))
                {
                    if (list.ContainsKey(item.Key))
                    {
                        list[item.Key] += item.Value;
                    }
                    else
                    {
                        list.Add(item.Key, item.Value);
                    }
                }
                
            }
            yield return null;
        }
        foreach (var item in list)
        {
            Debug.Log($"candy  x : {item.Key.x} y : {item.Key.y} temp : {item.Value}");
        }
        yield return null;
    }

    #region searchTemp
    private Dictionary<Candy, int> SearchHorizontal(Candy candy, int size)
    {
        var list = new Dictionary<Candy, int>();
        var map = CopyMap(candy, size);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                var temp = map[y, x];
                map[y, x] = map[y, x + 1];
                map[y, x + 1] = temp;
                var countTemp = CheakHorizontalTemp(map) 
                    + CheakVeritacalTemp(map);
                temp = map[y, x];
                map[y, x] = map[y, x + 1];
                map[y, x + 1] = temp;
                if (countTemp > 0)
                {
                    list.Add(_map[candy.y + y,candy.x + x], countTemp);
                }
            }
        }
        return list;
    }


    public int CheakHorizontalTemp(CandyColor[,] map)
    {
        var count = 0;
        var countTemp = 0;
        var color = CandyColor.None;
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (color == CandyColor.None)
                {
                    color = map[y, x];
                    count++;
                }
                else if (color == map[y, x])
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        countTemp++;
                    count = 0;
                    color = map[y, x];
                }
            }
        }
        return countTemp;
    }

    public int CheakVeritacalTemp(CandyColor[,] map)
    {
        var count = 0;
        var countTemp = 0;
        var color = CandyColor.None;
        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                if (color == CandyColor.None)
                {
                    color = map[y, x];
                    count++;
                }
                else if (color == map[y, x])
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                        countTemp++;
                    count = 0;
                    color = map[y, x];
                }
            }
        }
        return countTemp;
    }

    #endregion
    private int GetTem(Candy start)
    {
        var count = 0;
        foreach (var temp in _temps)
        {
            if (Cheak(start, temp))
            {
                count++;
            }
        }
        return count;
    }

    private bool Cheak(Candy center, int[,] temp)
    {
        var color = CandyColor.None;
        for (int y = 0; y < temp.GetLength(0); y++)
        {
            for (int x = 0; x < temp.GetLength(1); x++)
            {
                if (temp[y, x] == 1)
                {
                    if (color == CandyColor.None)
                    {
                        color = _map[center.y + y, center.x + x].color;
                    }
                    else if(color != _map[center.y + y, center.x + x].color)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
   
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
