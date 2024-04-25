using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class Server : NetworkManager
{
    [SerializeField] private HttpHolder _holder;
    [SerializeField] private PlayerState _botPrefab;
    [SerializeField] private RoundSession _session;

    private PlayerState _enemy;
    private PlayerState _player;

    private List<NetworkIdentity> _list = new List<NetworkIdentity>();

    public event System.Action<PlayerState, PlayerState> OnStart;
    public event System.Action<PlayerState> OnDisconect;

    public bool IsBot { get; private set; } = true;

    private void Update()
    {
        if (_player != null && _enemy != null)
        {
            _session.SetPlayer(_player, _enemy);
            OnStart?.Invoke(_player, _enemy);
            enabled = false;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var player = Instantiate(playerPrefab).GetComponent<PlayerState>();
        _list.Add(player.netIdentity);
        NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        _holder.SendGetMessange($"get_setting/{networkAddress}", (mess) => {
            var config = JsonUtility.FromJson<ServerConfigData>(mess);
            if (config.Bot)
            {
                IsBot = config.Bot;
                _enemy = Instantiate(_botPrefab).GetComponent<PlayerState>();
                NetworkServer.Spawn(_enemy.gameObject);
            }
            if (_player)
                _enemy = player;
            else
                _player = player;
        });
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        var player = _list.Find(x => conn.owned.Contains(x));
        if (player)
        {
            OnDisconect?.Invoke(player.GetComponent<PlayerState>());
            _list.Remove(player);
        }
        base.OnServerDisconnect(conn);
    }


    public void Spawn(GameObject asset)
    {
        NetworkServer.Spawn(asset);
    }

}
