using UnityEngine;
using kcp2k;
using System.Collections;

public class GameLoder : MonoBehaviour
{
    public const string GAMECONFIG = "gameConfig";
    public const string SESSION = "session";

    [SerializeField] private bool _isServer;
    [Header("Reference")]
    [SerializeField] private Server _server;
    [SerializeField] private KcpTransport _transport;
    [SerializeField] private Client _clientController;
    [SerializeField] private MachServer _serverController;

    private ServerData _serverData;

    private void Awake()
    {
        _server.OnDisconect += OnDisconect;
    }

    private void OnDestroy()
    {
        Debug.Log(JsonUtility.ToJson(_serverData));
        PlayerPrefs.SetString(SESSION, JsonUtility.ToJson(_serverData));
        _server.OnDisconect -= OnDisconect;
    }

    private void OnDisconect()
    {
        PlayerPrefs.SetString(SESSION, JsonUtility.ToJson(_serverData));
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(GAMECONFIG))
        {
            if (PlayerPrefs.GetString(GAMECONFIG) != null)
            {
                _serverData = JsonUtility.FromJson<ServerData>(PlayerPrefs.GetString(GAMECONFIG));
                _transport.Port = _serverData.Port;
                _server.networkAddress = _serverData.Adress;
                _clientController.StartClient();
            }
            PlayerPrefs.DeleteKey(GAMECONFIG);
        }
        else
        {
            _serverController.StartServer();
        }
    }

}
