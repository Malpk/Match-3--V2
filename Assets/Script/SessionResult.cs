[System.Serializable]
public struct SessionResult
{
    public int Stars;
    public int Coins;

    public SessionResult(int stars, int coins)
    {
        Stars = stars;
        Coins = coins;
    }
}
