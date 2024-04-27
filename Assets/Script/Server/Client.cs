using UnityEngine;
using Mirror;
using System.Collections;

public class Client : MonoBehaviour
{
    [SerializeField] private Server _client;
    [SerializeField] private RoundSessionUI _ui;
    [SerializeField] private PlayerHandler _controller;

    private Coroutine _corotine;
    private PlayerState _player;

    public bool IsStart { get; private set; } = false;

    private void OnEnable()
    {
        _client.OnPlayerConnect += OnConnect;
    }

    private void OnDisable()
    {
        _client.OnPlayerConnect -= OnConnect;
    }

    private void OnConnect()
    {
        if (_corotine == null)
            _corotine = StartCoroutine(WaitCreatePlayer());
    }


    private IEnumerator WaitCreatePlayer()
    {
        yield return new WaitWhile(() => !NetworkClient.localPlayer);
        _player = NetworkClient.localPlayer.GetComponent<PlayerState>();
        var user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString(UserAuto.USERKEY));
        _player.SetLoginCommand(user.Login);
        _controller.Play(_player);
        _corotine = null;
    }


    public void StartClient()
    {
        IsStart = true;
        Debug.Log("start");
        _client.StartClient();
        _ui.ShowLoad();
    }

    public void StopClient()
    {
        IsStart = false;
        _client.StopClient();
    }

}
