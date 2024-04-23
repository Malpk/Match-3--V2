using UnityEngine;
using UnityEngine.UI;

public class Matchmaking : MonoBehaviour
{
    [SerializeField] private int _rate;
    [SerializeField] private string _addPlayer;
    [SerializeField] private string _runMatchmaking;
    [Header("Reference")]
    [SerializeField] private Button _start;
    [SerializeField] private UserAuto _auto;
    [SerializeField] private HttpHolder _holder;

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
    }

    public void PlayMatchmaking()
    {
        if(!IsRun)
            _holder.SendGetMessange($"{_addPlayer}/{_auto.User.Login}/{_rate}", RunMatchmaking);
    }

    private void RunMatchmaking(string json)
    {
        Debug.Log(json);
        _holder.SendGetMessange($"{_runMatchmaking}", EnterToServer);
    }

    private void EnterToServer(string json)
    {
        if (!IsRun)
        {
            Debug.Log(json);
            IsRun = true;
        }
    }
}
