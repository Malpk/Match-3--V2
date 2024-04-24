using UnityEngine;
using Mirror;

public class Client : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private PlayerPanel _playerPanel;
    [SerializeField] private PlayerPanel _enemy;
    [SerializeField] private Server _client;
    [SerializeField] private PlayerHandler _player;
    [SerializeField] private WinMenu _menu;


    private void Awake()
    {
        NetworkClient.onConnectionQualityChanged += OnConnectCahnge;
    }

    private void OnConnectCahnge(ConnectionQuality arg1, ConnectionQuality arg2)
    {
        if (NetworkClient.localPlayer)
        {
            var player = NetworkClient.localPlayer.GetComponent<PlayerState>();
            player.SetNick(_name);
            _player.Play(player);
            _menu.SetPlayer(player);
        }
        else
        {
            _playerPanel.Bind(null);
        }
    }

    public void StartClient()
    {
        _client.StartClient();
    }

}
