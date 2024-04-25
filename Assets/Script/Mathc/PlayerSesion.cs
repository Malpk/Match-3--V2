public class PlayerSesion
{
    public int Score;
    public string Login;
    public string Adress;
    public PlayerState Player;

    public PlayerSesion(PlayerState player)
    {
        Score = 0;
        Player = player;
        Adress = Player.Adress;
    }

    public void Reconect(PlayerState player)
    {
        Player = player;
    }

    public void Enter()
    {
        Player.Enter();
    }

    public void Exit()
    {
        Player.ExitClient();
    }

    public void AddScore(int score)
    {
        Score += score;
    }
}
