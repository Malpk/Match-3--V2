using UnityEngine.Events;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerState : NetworkBehaviour
{
    [SerializeField] private int _score;
    [SerializeField] private string _name;

    public event System.Action OnEnter;
    public event System.Action OnExit;
    public event System.Action<int> OnSetRound;
    public event System.Action<float> OnRoundProgress;

    public string Name => _name;
    public int Score => _score;


    public void SetNick(string nick)
    {
        _name = nick;
        SetServerData(_name);
    }

    [Server]
    public void AddScore(int score)
    {
        if(isServer)
            _score += score;
    }


    [Command]
    private void SetServerData(string nick)
    {
        _name = nick;
    }

    [Command]
    public void SwipeBoard(string nick)
    {
        _name = nick;
    }

    [ClientRpc]
    public void Enter()
    {
        OnEnter?.Invoke();
    }

    [ClientRpc]
    public void Exit()
    {
        OnExit?.Invoke();
    }

}
