using UnityEngine;
using Mirror;
using GameVanilla.Game.Scenes;

public class MachServer : MonoBehaviour
{
    [SerializeField] private Server _server;
    [SerializeField] private GameScene _scene;
    [SerializeField] private RoundSession _session;

    private void Awake()
    {
        _server.OnAddPlayer += OnAddPlayer;
    }

    private void OnDestroy()
    {
        _server.OnAddPlayer -= OnAddPlayer;
    }

    private void OnAddPlayer(PlayerState player, PlayerState enemy)
    {
        if (_server.numPlayers > 0)
        {
            _session.StartGame();
            _scene.Play();
        }
    }

    private void Start()
    {
        _server.StartServer();
    }
}
