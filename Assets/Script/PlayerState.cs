using UnityEngine;
using Mirror;
using GameVanilla.Game.Common;

public class PlayerState : NetworkBehaviour
{
    [SerializeField] private bool _isWin;
    [SyncVar][SerializeField] private string _login;

    private GameBoard _board;

    public event System.Action OnEnter;
    public event System.Action OnExit;
    public event System.Action<int> OnSetRound;
    public event System.Action<float> OnRoundProgress;
    public event System.Action<PlayerState, PlayerState, SessionResult> OnCompliteGame;

    public string Adress { get; private set; }

    public int Score { get; private set; }
    public bool IsWin => _isWin;
    public bool Disconect { get; private set; }

    public event System.Action<string> OnLogin;

    public void SetWin(bool win)
    {
        _isWin = win;
        SetClientWin(win);
    }

    [ClientRpc]
    private void SetClientWin(bool win)
    {
        _isWin = win;
    }

    [Server]
    public void SetAdress(string adress)
    {
        Adress = adress;
    }

    public void SetLogin(string nick)
    {
        _login = nick;
    }

    public void SetBoard(GameBoard board)
    {
        _board = board;
    }

    [ClientRpc]
    public void SetScore(int score)
    {
        Score = score;
    }

    [Command]
    public void Swipe(Tile select, Tile tile)
    {
        _board.InputBoard(tile, select);
    }

    [Command]
    public void SwipeBoard(string nick)
    {
        _login = nick;
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

    [ClientRpc]
    public void CompliteGame(PlayerState enemy, SessionResult result)
    {
        OnCompliteGame?.Invoke(this, enemy, result);
    }

    public void SetConnectStatus(bool disconect)
    {
        Disconect = disconect;
    }
}
