using UnityEngine;
using kcp2k;
using System.Collections;
using Mirror;

public class GameLoder : MonoBehaviour
{
    public const string GAMECONFIG = "gameConfig";
    public const string SESSION = "session";

    [SerializeField] private bool _isServer;
    [Header("Reference")]
    [SerializeField] private Server _server;
    [SerializeField] private TelepathyTransport _transport;
    [SerializeField] private Client _clientController;
    [SerializeField] private MachServer _serverController;

    private ServerData _serverData;

    private void Awake()
    {
        _server.OnDisconect += OnDisconect;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetString(SESSION, JsonUtility.ToJson(_serverData));
        _server.OnDisconect -= OnDisconect;
        if (_clientController.IsStart)
            _clientController.StopClient();
        if (_serverController.IsStart)
            _serverController.StopServer();
    }

    private void OnDisconect()
    {
        PlayerPrefs.SetString(SESSION, JsonUtility.ToJson(_serverData));
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(GAMECONFIG) && !_isServer)
        {
            var data = PlayerPrefs.GetString(GAMECONFIG);
            Debug.Log(data);
            try
            {
                _serverData = JsonUtility.FromJson<ServerData>(data);
                _transport.Port = _serverData.Port;
                _server.networkAddress = _serverData.Adress;
                _clientController.StartClient();
            }
            catch 
            {
                Debug.Log(data);
            }
        }
        else
        {
            _serverController.StartServer();
        }
    }

}
