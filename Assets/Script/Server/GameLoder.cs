using UnityEngine;
using Mirror;
using kcp2k;

public class GameLoder : MonoBehaviour
{
    public const string GAMECONFIG = "gameConfig";

    [SerializeField] private bool _isServer;
    [Header("Reference")]
    [SerializeField] private NetworkManager _network;
    [SerializeField] private KcpTransport _transport;
    [SerializeField] private Client _client;
    [SerializeField] private MachServer _server;


    private void Start()
    {
        if (PlayerPrefs.HasKey(GAMECONFIG))
        {
            if (PlayerPrefs.GetString(GAMECONFIG) != null)
            {
                var server = JsonUtility.FromJson<ServerData>(PlayerPrefs.GetString(GAMECONFIG));
                Debug.Log(server != null);
                _transport.Port = (ushort)server.Port;
                _network.networkAddress = server.Adress;
                _client.StartClient();
                _server.gameObject.SetActive(false);
            }
            PlayerPrefs.DeleteKey(GAMECONFIG);
        }
        else
        {
            _server.StartServer();
            _client.gameObject.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey(GAMECONFIG);
    }
}
