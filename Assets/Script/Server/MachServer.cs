using UnityEngine;
using GameVanilla.Game.Scenes;
using GameVanilla.Game.Common;

public class MachServer : MonoBehaviour
{
    [SerializeField] private MathcBot _bot;
    [SerializeField] private Server _server;
    [SerializeField] private GameScene _scene;
    [SerializeField] private GameBoard _board;
    [SerializeField] private RoundSession _session;

    private void Awake()
    {
        _server.OnAddPlayer += OnAddPlayer;
        _server.OnDisconect += OnDisconect;
    }

    private void OnDestroy()
    {
        _server.OnAddPlayer -= OnAddPlayer;
        _server.OnDisconect -= OnDisconect;
    }

    private void OnDisconect(PlayerState obj)
    {
        _session.StopGame();
        _scene.Play();
    }

    private void OnAddPlayer(PlayerState player, PlayerState enemy)
    {
        if (_server.numPlayers > 0)
        {
            player.SetBoard(_board);
            if (_server.IsBot)
                _bot.SetPlayer(enemy);
            enemy.SetBoard(_board);
            _session.StartGame();
            _scene.Play();
        }
    }

    private void Start()
    {
        _server.StartServer();
    }
}
