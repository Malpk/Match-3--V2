using Mirror;

[System.Serializable]
public class SessionResult
{
    public string Player;
    public string Enemy;
    public bool Win;
    public int Stars;
    public int Coins;
    public int Score;

    public SessionResult(int stars, int coins)
    {
        Stars = stars;
        Coins = coins;
    }
}
