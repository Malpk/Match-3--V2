using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Matchmaking : MonoBehaviour
{
    [SerializeField] private int _gameSceneID;
    [SerializeField] private int _rate;
    [SerializeField] private string _addPlayer;
    [SerializeField] private string _runMatchmaking;
    [Header("Reference")]
    [SerializeField] private Button _start;
    [SerializeField] private Button _reconnect;
    [SerializeField] private UserAuto _auto;
    [SerializeField] private HttpHolder _holder;

    private ServerData _reconnectData;

    public bool IsRun { get; private set; } = false;

    private void Reset()
    {
        _rate = 100;
        _addPlayer = "join_queue";
        _runMatchmaking = "matchaking";
    }

    private void Awake()
    {
        _start.onClick.AddListener(PlayMatchmaking);
        _reconnect.onClick.AddListener(Reconect);
        _reconnect.interactable = false;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(GameLoder.SESSION))
        {
            var session = JsonUtility.FromJson<ServerData>(PlayerPrefs.GetString(GameLoder.SESSION));
            if (session != null)
            {
                _holder.SendGetMessange($"reconect/{session.Adress}", (mess) =>
                {
                    Debug.Log(mess);
                    _reconnectData = JsonUtility.FromJson<ServerData>(mess);
                    _reconnect.interactable = _reconnectData != null;
                });
            }
            PlayerPrefs.DeleteKey(GameLoder.SESSION);
        }
    }

    public void PlayMatchmaking()
    {
        if(!IsRun)
            _holder.SendGetMessange($"{_addPlayer}/{_auto.User.Login}/{_rate}", RunMatchmaking);
    }

    private void RunMatchmaking(string json)
    {
        Debug.Log(json);
        _holder.SendGetMessange($"{_runMatchmaking}/{_auto.User.Login}", EnterToServer);
    }

    private void Reconect()
    {
        EnterToServer(JsonUtility.ToJson(_reconnectData));
    }

    private void EnterToServer(string json)
    {
        if (!IsRun)
        {
            PlayerPrefs.SetString(GameLoder.GAMECONFIG, json);
            SceneManager.LoadScene(_gameSceneID);
            IsRun = true;
        }
    }
}
