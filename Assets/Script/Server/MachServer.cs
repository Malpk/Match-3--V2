using UnityEngine;
using GameVanilla.Game.Scenes;
using GameVanilla.Game.Common;
using kcp2k;
using System.Collections;

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
    private Coroutine _complite;

    private void Awake()
    {
        _server.OnStart += OnAddPlayer;
        _server.OnDisconect += OnDisconect;
        _session.OnWin += OnSave;
    }

    private void OnDestroy()
    {
        _server.OnStart -= OnAddPlayer;
        _server.OnDisconect -= OnDisconect;
        _session.OnWin -= OnSave;
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

    public void OnSave(string progress)
    {
        _holder.SendGetMessange($"save_result/{progress}", (mess) => {
            Debug.Log(mess);
        });
    }
    private void OnDisconect()
    {
        if (_server.numPlayers == 0)
        {
            if(_complite == null)
                _complite = StartCoroutine(Complite());
        }
    }

    private IEnumerator Complite()
    {
        Debug.Log(_session.IsComplite);
        yield return new WaitWhile(() => !_session.IsComplite);
        _holder.SendGetMessange($"complite/{_server.networkAddress}", (mess) =>
        {
            Debug.Log(mess);
        });
        _complite = null;
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
