using UnityEngine;
using Mirror;

public class Client : MonoBehaviour
{
    [SerializeField] private Server _client;
    [SerializeField] private RoundSessionUI _ui;
    [SerializeField] private PlayerHandler _controller;

    private PlayerState _player;
    public bool IsStart { get; private set; } = false;

    private void OnEnable()
    {
        NetworkClient.onConnectionQualityChanged += OnConnectCahnge;
    }

    private void OnDisable()
    {
        NetworkClient.onConnectionQualityChanged -= OnConnectCahnge;
    }

    private void OnConnectCahnge(ConnectionQuality arg1, ConnectionQuality arg2)
    {
        Debug.Log(_player);
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
        IsStart = true;
        _client.StartClient();
        _ui.ShowLoad();
        Debug.Log("Start");
    }

    public void StopClient()
    {
        IsStart = false;
        _client.StopClient();
    }

}
