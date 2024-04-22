using UnityEngine;
using TMPro;
using Mirror;

public class PlayerPanel : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _score;

    [ClientRpc]
    public void Bind(string name)
    {
        _name.SetText(name);
    }

    [ClientRpc]
    public void SetScore(int score)
    {
        _score.SetText(score.ToString());
    }
}