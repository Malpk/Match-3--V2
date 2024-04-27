using UnityEngine;

public class PlayerSesion
{
    public int Score;
    public bool IsBot;
    public string Login;
    public string Adress;
    public PlayerState Player;

    private bool _isEnter;

    public PlayerSesion(PlayerState player, bool bot = false)
    {
        Score = 0;
        IsBot = bot;
        Player = player;
        Login = player.Login;
        Adress = Player.Adress;
        Player.OnSetLogin += OnSetLogin;
        Player.OnDisconnect += PlayerDisconnect;
    }

    ~PlayerSesion()
    {
        if (Player)
            PlayerDisconnect();
    }

    private void OnSetLogin()
    {
        Login = Player.Login;
    }

    public void Reconect(PlayerState player)
    {
        Player = player;
        if (_isEnter)
            Enter();
        else
            Exit();
    }

    public void PlayerDisconnect()
    {
        Player.OnDisconnect -= PlayerDisconnect;
        Player.OnSetLogin -= OnSetLogin;
    }

    public void Enter()
    {
        _isEnter = true;
        Player?.Enter();
    }

    public void Exit()
    {
        _isEnter = false;
        try
        {
            Player?.ExitClient();
        }
        catch {
            Debug.Log("player disconnect");
        }

    }

    public void AddScore(int score)
    {
        Score += score;
    }
}
