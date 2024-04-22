using UnityEngine;
using Mirror;
using TMPro;

public class Client : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private PlayerPanel _playerPanel;
    [SerializeField] private PlayerPanel _enemy;
    [SerializeField] private Server _client;
    [SerializeField] private PlayerHandler _player;


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
        }
        else
        {
            _playerPanel.Bind(null);
        }
    }

    private void Start()
    {
        _client.StartClient();
    }

}
