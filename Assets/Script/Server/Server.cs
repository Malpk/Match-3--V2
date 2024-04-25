using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class Server : NetworkManager
{
    [SerializeField] private HttpHolder _holder;
    [SerializeField] private PlayerState _botPrefab;
    [SerializeField] private RoundSession _session;

    private List<string> _adresses = new List<string>();
    private PlayerState _enemy;
    private PlayerState _player;

    private List<PlayerState> _list = new List<PlayerState>();

    public event System.Action<PlayerState, PlayerState> OnStart;
    public event System.Action OnDisconect;

    public bool IsBot { get; private set; } = true;
    public bool IsReady => enabled;


    public override void Update()
    {
        base.Update();
        if (_player != null && _enemy != null)
        {
            _session.SetPlayer(_player, _enemy);
            _session.OnWin += OnWin;
            OnStart?.Invoke(_player, _enemy);
            enabled = false;
        }
    }

    private void OnWin(string result)
    {
        _player = null;
        _enemy = null;
        enabled = true;
        _session.OnWin -= OnWin;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (!AddNewPlayer(conn))
            Reconnect(conn);
    }

    private bool AddNewPlayer(NetworkConnectionToClient conn)
    {
        if (!IsReady)
            return false;
        var player = AddPlayer(conn);
        _holder.SendGetMessange($"get_setting/{networkAddress}", (mess) => {
            var config = JsonUtility.FromJson<ServerConfigData>(mess);
            _adresses.Add(player.Adress);
            if (config.Bot)
            {
                IsBot = config.Bot;
                _enemy = Instantiate(_botPrefab).GetComponent<PlayerState>();
                _enemy.SetLogin("Противник");
                NetworkServer.Spawn(_enemy.gameObject);
            }
            if (_player)
                _enemy = player;
            else
                _player = player;
        });
        return true;
    }

    private PlayerState AddPlayer(NetworkConnectionToClient conn)
    {
        var player = Instantiate(playerPrefab).GetComponent<PlayerState>();
        NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        player.SetAdress(conn.address);
        _list.Add(player);
        return player;
    }

    private void Reconnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"Reconect {conn} : {CheakAdress(conn.address)}");
        if (CheakAdress(conn.address))
        {
            var player = AddPlayer(conn);
            _session.Reconect(player);
        }
    }

    private bool CheakAdress(string target)
    {
        foreach (var adress in _adresses)
        {
            if (adress == target)
            {
                return true;
            }
        }
        return false;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        var player = _list.Find(x => conn.owned.Contains(x.netIdentity));
        _list.Remove(player);
        base.OnServerDisconnect(conn);
    }


    public void Spawn(GameObject asset)
    {
        NetworkServer.Spawn(asset);
    }

}
