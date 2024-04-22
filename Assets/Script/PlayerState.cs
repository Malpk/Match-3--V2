using UnityEngine;
using Mirror;
using GameVanilla.Game.Common;

public class PlayerState : NetworkBehaviour
{
    [SyncVar] [SerializeField] private bool _isWin;
    [SerializeField] private int _score;
    [SerializeField] private string _name;

    private GameBoard _board;

    public event System.Action OnEnter;
    public event System.Action OnExit;
    public event System.Action<int> OnSetRound;
    public event System.Action<float> OnRoundProgress;

    public string Name => _name;
    public bool IsWin => _isWin;
    public int Score => _score;


    public void SetNick(string nick)
    {
        _name = nick;
        SetServerData(_name);
    }

    public void SetBoard(GameBoard board)
    {
        _board = board;
    }

    [Server]
    public void AddScore(int score)
    {
        if(isServer)
            _score += score;
    }

    [Command]
    public void Swipe(Tile select, Tile tile)
    {
        _board.InputBoard(tile, select);
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


    public void Enter()
    {
        OnEnter?.Invoke();
        EnterClient();
    }

    [ClientRpc]
    private void EnterClient()
    {
        OnEnter?.Invoke();
    }

    [ClientRpc]
    public void Exit()
    {
        OnExit?.Invoke();
    }

}
