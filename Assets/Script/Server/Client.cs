using UnityEngine;
using Mirror;

public class Client : MonoBehaviour
{
    [SerializeField] private Server _client;
    [SerializeField] private RoundSessionUI _ui;
    [SerializeField] private PlayerHandler _controller;

    private PlayerState _player;

    private void Awake()
    {
        NetworkClient.onConnectionQualityChanged += OnConnectCahnge;
    }

    private void OnConnectCahnge(ConnectionQuality arg1, ConnectionQuality arg2)
    {
        if (NetworkClient.localPlayer && !_player)
        {
            _player = NetworkClient.localPlayer.GetComponent<PlayerState>();
            var user =JsonUtility.FromJson<UserData>(PlayerPrefs.GetString(UserAuto.USERKEY));
            _player.SetLoginCommand(user.Login);
            _controller.Play(_player);
        }
    }

    public void StartClient()
    {
        _client.StartClient();
        _ui.ShowLoad();
    }

}
