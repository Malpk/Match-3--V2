using Mirror;
using GameVanilla.Game.Common;

public class PlayerState : NetworkBehaviour
{
    private GameBoard _board;

    public event System.Action OnEnter;
    public event System.Action OnExit;
    public event System.Action<int> OnSetRound;
    public event System.Action<float> OnRoundProgress;
    public event System.Action<string> OnSetLogin;

    public string Adress { get; private set; }

    [Server]
    public void SetAdress(string adress)
    {
        Adress = adress;
    }

    [Command]
    public void SetLogin(string login)
    {
        OnSetLogin?.Invoke(login);
    }

    [Command]
    public void Swipe(Tile select, Tile tile)
    {
        _board.InputBoard(tile, select);
    }


    public void Enter()
    {
        OnEnter?.Invoke();
        EnterClient();
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
