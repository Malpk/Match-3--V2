using Mirror;

public class RoundClientSession : NetworkBehaviour
{
    public event System.Action<int> OnRaund;
    public event System.Action<float> OnProgress;

    [ClientRpc]
    public void SetProgress(float progress)
    {
        OnProgress?.Invoke(progress);
    }

    [ClientRpc]
    public void SetRound(int round)
    {
        OnRaund?.Invoke(round);
    }
}
