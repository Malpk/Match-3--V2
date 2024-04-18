using UnityEngine;
using Mirror;

public class Client : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private PlayerPanel _playerPanel;
    [SerializeField] private PlayerPanel _enemy;
    [SerializeField] private MathcClient _client;

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
            _playerPanel.Bind(player);
        }
        else
        {
            _playerPanel.Bind(null);
        }

        foreach (var item in NetworkClient.spawned)
        {
            if (item.Value != NetworkClient.localPlayer)
            {
                if (item.Value.TryGetComponent(out PlayerState state))
                {
                    _enemy.Bind(state);
                    return;
                }
            }
        }
        _enemy.Bind(null);
    }

    private void Start()
    {
        _client.StartClient();
    }



}
