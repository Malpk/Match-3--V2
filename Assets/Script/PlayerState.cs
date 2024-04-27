using Mirror;
using GameVanilla.Game.Common;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    private GameBoard _board;

    public event System.Action OnEnter;
    public event System.Action OnExit;
    public event System.Action<int> OnSetRound;
    public event System.Action<float> OnRoundProgress;
    public event System.Action OnSetLogin;
    public event System.Action OnDisconnect;


    public string Adress { get; private set; }
    public string Login { get; private set; }

    [Server]
    public void SetAdress(string adress)
    {
        Adress = adress;
    }

    [Command]
    public void SetLoginCommand(string login)
    {
        Login = login;
        OnSetLogin?.Invoke();
    }

    public void SetLogin(string login)
    {
        Login = login;
    }

    [Command]
    public void Swipe(Tile select, Tile tile)
    {
        _board.Swipe(tile, select);
    }


    public void Enter()
    {
        OnEnter?.Invoke();
        try
        {
            EnterClient();
        }
        catch
        {
            Debug.Log("player disconnect");
        }

    }

    public void Disconnect()
    {
        OnDisconnect?.Invoke();
    }

    public void SetBoard(GameBoard board)
    {
        _board = board;
    }

    #region Server

    [ClientRpc]
    private void EnterClient()
    {
        OnEnter?.Invoke();
    }

    [ClientRpc]
    public void ExitClient()
    {
        OnExit?.Invoke();
    }
    #endregion

    

}
