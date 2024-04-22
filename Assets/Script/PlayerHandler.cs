using System;
using UnityEngine;

using GameVanilla.Game.Common;
using GameVanilla.Game.UI;
using GameVanilla.Game.Scenes;

using System.Collections.Generic;
using Mirror;


public class PlayerHandler : NetworkBehaviour
{
    [SerializeField] private GameBoard _board;
    [SerializeField] private GameScene _scene;

    private bool _blockInput;

    private bool drag;
    private GameObject selectedTile;

    private PlayerState _player;

    private List<GameObject> tiles => _board.Tiles;
    private Level level => _board.level;

    private void Reset()
    {
        enabled = false;
    }

    public void Play(PlayerState player)
    {
        if (_player)
        {
            _player.OnEnter -= OnEnter;
            _player.OnExit -= OnExit;
        }
        _player = player;
        _player.OnEnter += OnEnter;
        _player.OnExit += OnExit;
    }

    public void Stop()
    {
        _player.OnEnter -= OnEnter;
        _player.OnExit -= OnExit;
        _player = null;
        enabled = false;
    }

    private void OnEnter()
    {
        enabled = true;
    }

    private void OnExit()
    {
        enabled = false;
    }

    private void Update()
    {
        if (_scene.BoosterMode)
        {
            if (_scene.CurrentBoosterButton.boosterType == BoosterType.Switch)
            {
                HandleSwitchBoosterInput(_scene.CurrentBoosterButton);
            }
            else
            {
                HandleBoosterInput(_scene.CurrentBoosterButton);
            }
        }
        else if (!_blockInput)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (_board.InputLocked)
            return;
        if (_board.CurrentlySwapping)
            return;
        if (_board.CurrentlyAwarding)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            drag = true;
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (!hit)
                return;
            if (hit.collider.CompareTag("Tile"))
            {
                var idx = tiles.FindIndex(x => x == hit.collider.gameObject);
                if (level.tiles[idx] != null && level.tiles[idx].elementType == ElementType.Ice)
                {
                    return;
                }

                if (hit.collider.GetComponent<SpecialBlock>() != null)
                {
                    return;
                }

                selectedTile = hit.collider.gameObject;
                selectedTile.GetComponent<Animator>().SetTrigger("Pressed");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            drag = false;
            if (selectedTile != null && selectedTile.GetComponent<Animator>() != null && selectedTile.gameObject.activeSelf)
            {
                selectedTile.GetComponent<Animator>().SetTrigger("Unpressed");
            }
        }

        if (drag && selectedTile != null)
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            var tile = hit ? hit.collider.GetComponent<Tile>() : null;
            if (!tile)
                return;
            if (tile.gameObject != selectedTile)
            {
                if (tile.GetComponent<SpecialBlock>() != null)
                    return;
                if (selectedTile.GetComponent<Animator>() != null && selectedTile.gameObject.activeSelf)
                {
                    selectedTile.GetComponent<Animator>().SetTrigger("Unpressed");
                }

                var idx = tiles.FindIndex(x => x == tile.gameObject);
                if (level.tiles[idx] != null && level.tiles[idx].elementType == ElementType.Ice)
                    return;

                var idxSelected = tiles.FindIndex(x => x == selectedTile);
                var xSelected = idxSelected % level.width;
                var ySelected = idxSelected / level.width;
                var idxNew = tiles.FindIndex(x => x == tile.gameObject);
                var xNew = idxNew % level.width;
                var yNew = idxNew / level.width;
                if (Math.Abs(xSelected - xNew) > 1 || Math.Abs(ySelected - yNew) > 1)
                    return;
                _player.Swipe(selectedTile.GetComponent<Tile>(), tile);
                selectedTile = null;
            }
        }
    }

    public void HandleSwitchBoosterInput(BuyBoosterButton button)
    {
        if (Input.GetMouseButtonDown(0))
        {
            drag = true;
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Tile"))
            {
                selectedTile = hit.collider.gameObject;
            }
            else
            {
                _scene.DisableBoosterMode();
                return;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            drag = false;
        }

        if (drag && selectedTile != null)
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject != selectedTile)
            {
                var selectedTileCopy = selectedTile;
                selectedTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
                LeanTween.move(selectedTile, hit.collider.gameObject.transform.position, 0.25f).setOnComplete(
                    () =>
                    {
                        selectedTileCopy.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        _scene.DisableBoosterMode();
                        _board.HandleMatches(true);
                        _board.ConsumeBooster(button);
                    });
                LeanTween.move(hit.collider.gameObject, selectedTile.transform.position, 0.25f);

                var tileA = hit.collider.gameObject;
                var tileB = selectedTile;
                var idxA = tiles.FindIndex(x => x == tileA);
                var idxB = tiles.FindIndex(x => x == tileB);
                tiles[idxA] = tileB;
                tiles[idxB] = tileA;

                tileA.GetComponent<Tile>().x = idxB % level.width;
                tileA.GetComponent<Tile>().y = idxB / level.width;
                tileB.GetComponent<Tile>().x = idxA % level.width;
                tileB.GetComponent<Tile>().y = idxA / level.width;

                selectedTile = null;

            }
        }
    }
    public void HandleBoosterInput(BuyBoosterButton button)
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Tile"))
            {
                if (hit.collider.GetComponent<Unbreakable>() != null ||
                    hit.collider.GetComponent<Collectable>() != null)
                {
                    return;
                }

                var tile = hit.collider.GetComponent<Tile>();
                Booster booster = null;
                switch (button.boosterType)
                {
                    case BoosterType.Lollipop:
                        booster = new LollipopBooster();
                        break;

                    case BoosterType.Bomb:
                        booster = new BombBooster();
                        break;

                    case BoosterType.ColorBomb:
                        booster = new ColorBombBooster();
                        break;
                }

                if (booster != null)
                {
                    booster.Resolve(_board, tile.gameObject);
                    _board.ConsumeBooster(button);
                   _board.ApplyGravity();
                }

                _scene.DisableBoosterMode();

                selectedTile = hit.collider.gameObject;
            }
            else
            {
                _scene.DisableBoosterMode();
            }
        }
    }
}
