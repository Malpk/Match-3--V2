using UnityEngine;
using GameVanilla.Game.Scenes;
using GameVanilla.Game.Common;
using kcp2k;

public class MachServer : MonoBehaviour
{
    [SerializeField] private MathcBot _bot;
    [SerializeField] private Server _server;
    [SerializeField] private GameScene _scene;
    [SerializeField] private GameBoard _board;
    [SerializeField] private RoundSession _session;
    [SerializeField] private HttpHolder _holder;
    [SerializeField] private KcpTransport _transport;

    private bool _isStart = false;

    private void Awake()
    {
        _server.OnStart += OnAddPlayer;
        _session.OnWin += ComliteSession;
    }

    private void OnDestroy()
    {
        _server.OnStart -= OnAddPlayer;
        _session.OnWin -= ComliteSession;
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

    public void ComliteSession(string json)
    {
        _holder.SendGetMessange($"complite/{_server.networkAddress}/{json}", (mess) => {
            Debug.Log(mess);
        });
    }

    public void StartServer()
    {
        if (!_isStart)
        {
            _isStart = true;
            _server.StartServer();
            _holder.SendGetMessange($"add_server/{_server.networkAddress}/{_transport.port}", (mess) => {
                Debug.Log(mess);
                var data = JsonUtility.FromJson<ServerData>(mess);
                if (data != null)
                {
                    _server.networkAddress = data.Adress;
                    _transport.port = data.Port;
                }
            });
        }
    }

    private void OnApplicationQuit()
    {
        if(_isStart)
            _holder.SendGetMessange($"remove_server/{_server.networkAddress}", (mess) => Debug.Log(mess));
    }

}
