using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Matchmaking : MonoBehaviour
{
    [SerializeField] private string _gameSceneID;
    [SerializeField] private int _rate;
    [SerializeField] private string _addPlayer;
    [SerializeField] private string _runMatchmaking;
    [Header("Reference")]
    [SerializeField] private Button _start;
    [SerializeField] private Button _reconnect;
    [SerializeField] private UserAuto _auto;
    [SerializeField] private HttpHolder _holder;
    [SerializeField] private TextMeshProUGUI _timer;

    private float progress = 0f;
    private ServerData _reconnectData;

    public bool IsRun { get; private set; } = false;

    private void Reset()
    {
        enabled = false;
        _rate = 100;
        _addPlayer = "join_queue";
        _runMatchmaking = "matchaking";
        _gameSceneID = "GameScene";
    }

    private void Awake()
    {
        _start.onClick.AddListener(PlayMatchmaking);
        _reconnect.onClick.AddListener(Reconect);
        _reconnect.interactable = false;
        _timer.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(GameLoder.SESSION))
        {
            try
            {
                var session = JsonUtility.FromJson<ServerData>(PlayerPrefs.GetString(GameLoder.SESSION));
                _holder.SendGetMessange($"reconect/{session.Adress}/{_auto.User.Login}", (mess) =>
                {
                    Debug.Log(mess);
                    _reconnectData = JsonUtility.FromJson<ServerData>(mess);
                    _reconnect.interactable = _reconnectData != null;
                });
            }
            catch
            {
                Debug.Log("Sessia is not found");
            }
            PlayerPrefs.DeleteKey(GameLoder.SESSION);
        }
    }

    private void Update()
    {
        progress += Time.deltaTime;
        var time = (int)progress;
        _timer.SetText($"{GetFormat(time / 60)}:{GetFormat(time % 60)}");
    }

    private string GetFormat(int value)
    {
        var text = value.ToString();
        while (text.Length < 2)
        {
            text = "0" + text;
        }
        return text;
    }

    public void PlayMatchmaking()
    {
        if (!IsRun)
        {
            IsRun = true;
            _timer.SetText("0:00");
            progress = 0;
            _timer.gameObject.SetActive(true);
            enabled = true;
            _holder.SendGetMessange($"{_addPlayer}/{_auto.User.Login}/{_rate}", RunMatchmaking);
        }
    }

    private void RunMatchmaking(string json)
    {
        Debug.Log("run matchmaking");
        _holder.SendGetMessange($"{_runMatchmaking}/{_auto.User.Login}", EnterToServer);
    }

    private void Reconect()
    {
        EnterToServer(JsonUtility.ToJson(_reconnectData));
    }

    private void EnterToServer(string json)
    {
        try
        {
            var data = JsonUtility.FromJson<ServerData>(json);
            if (data.Adress != _runMatchmaking)
            {
                PlayerPrefs.SetString(GameLoder.GAMECONFIG, json);
                SceneManager.LoadScene(_gameSceneID);
            }
        }
        catch 
        {
            Debug.Log(json);
        }
        IsRun = false;
        enabled = false;
        _timer.gameObject.SetActive(false);
    }
}
