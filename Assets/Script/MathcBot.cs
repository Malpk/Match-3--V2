using System.Collections;
using System.Collections.Generic;
using GameVanilla.Game.Common;
using UnityEngine;

public class MathcBot : MonoBehaviour
{
    [SerializeField] private Transform _tileHolder;

    [SerializeField] private List<Tile> _tiles = new List<Tile>();

    public void Play()
    {
        _tiles.Clear();
        _tiles.AddRange(_tileHolder.GetComponentsInChildren<Tile>());
    }

}
