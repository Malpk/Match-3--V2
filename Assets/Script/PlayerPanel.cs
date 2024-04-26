using UnityEngine;
using TMPro;
using Mirror;

public class PlayerPanel : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _score;

    [TargetRpc]
    public void SetLogin(NetworkConnectionToClient client, string name)
    {
        _name.SetText(name);
    }

    [ClientRpc]
    public void SetScore(int score)
    {
        _score.SetText(score.ToString());
    }
}
