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
        Adress = Player.Adress;
    }

    public void Reconect(PlayerState player)
    {
        Player = player;
        if (_isEnter)
            Enter();
        else
            Exit();
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
