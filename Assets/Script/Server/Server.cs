using Mirror;
using UnityEngine;

public class Server : NetworkManager
{
    [SerializeField] private PlayerState _botPrefab;
    [SerializeField] private RoundSession _session;

    private PlayerState _bot;

    public event System.Action<PlayerState, PlayerState> OnAddPlayer;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var player = Instantiate(playerPrefab).GetComponent<PlayerState>();
        _bot = Instantiate(_botPrefab).GetComponent<PlayerState>();
        NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        NetworkServer.Spawn(_bot.gameObject);
        _session.SetPlayer(player,_bot);
        OnAddPlayer?.Invoke(player, _bot);
    }
}
