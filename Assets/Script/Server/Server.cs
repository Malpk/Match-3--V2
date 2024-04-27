using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class Server : NetworkManager
{
    [SerializeField] private HttpHolder _holder;
    [SerializeField] private PlayerState _botPrefab;
    [SerializeField] private RoundSession _session;
    [SerializeField] private RoundSessionUI _hud;

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
            _session.SetPlayer(_player, _enemy, IsBot);
            _session.OnWin += OnWin;
            OnStart?.Invoke(_player, _enemy);
            _hud.HideLoad();
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
                UpdateLoginClient(_player, _enemy);
                NetworkServer.Spawn(_enemy.gameObject);
            }
        });
        if (_player)
        {
            _enemy = player;
            IsBot = false;
            _enemy.OnSetLogin += UpdateLogin;
        }
        else
        {
            _player = player;
            _player.OnSetLogin += UpdateLogin;
        }
        return true;
    }

    private void UpdateLogin()
    {
        if(_player)
            UpdateLoginClient(_player, _enemy);
        //if (_enemy && !IsBot)
        //    UpdateLoginClient(_enemy, _player);
    }

    private void UpdateLoginClient(PlayerState player, PlayerState enemy)
    {
        _hud.Player.SetLogin(player.netIdentity.connectionToClient, player.Login);
        if(enemy)
            _hud.Enemy.SetLogin(player.netIdentity.connectionToClient, enemy.Login);
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
        player.Disconnect();
        base.OnServerDisconnect(conn);
        OnDisconect?.Invoke();
    }


    public void Spawn(GameObject asset)
    {
        NetworkServer.Spawn(asset);
    }

}
