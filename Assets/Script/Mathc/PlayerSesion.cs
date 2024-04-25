public class PlayerSesion
{
    public int Score;
    public string Adress;
    public PlayerState Player;

    public PlayerSesion(PlayerState player)
    {
        Score = 0;
        Player = player;
        Adress = Player.Adress;
        Player.SetScore(0);
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
        Player.Exit();
    }

    public void AddScore(int score)
    {
        Score += score;
        Player?.SetScore(score);
    }
}
