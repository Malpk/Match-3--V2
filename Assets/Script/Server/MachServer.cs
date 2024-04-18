using UnityEngine;
using Mirror;

public class MachServer : MonoBehaviour
{
    [SerializeField] private Server _server;
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
        }
    }

    private void Start()
    {
        _server.StartServer();
    }
}
