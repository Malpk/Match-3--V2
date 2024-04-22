using Mirror;
using UnityEngine;

public class Server : NetworkManager
{
    [SerializeField] private PlayerState _botPrefab;
    [SerializeField] private RoundSession _session;

    private PlayerState _bot;

    private NetworkIdentity _player;

    public event System.Action<PlayerState, PlayerState> OnAddPlayer;
    public event System.Action<PlayerState> OnDisconect;

    public bool IsBot { get; private set; } = true;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var player = Instantiate(playerPrefab).GetComponent<PlayerState>();
        _bot = Instantiate(_botPrefab).GetComponent<PlayerState>();
        NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        NetworkServer.Spawn(_bot.gameObject);
        _session.SetPlayer(player, _bot);
        OnAddPlayer?.Invoke(player, _bot);
        _player = player.GetComponent<NetworkIdentity>();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (!conn.owned.Contains(_player))
            OnDisconect?.Invoke(_player.GetComponent<PlayerState>());
        base.OnServerDisconnect(conn);
    }


    public void Spawn(GameObject asset)
    {
        NetworkServer.Spawn(asset);
    }

}
