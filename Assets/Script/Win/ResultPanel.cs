using UnityEngine;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stars;
    [SerializeField] private TextMeshProUGUI _coins;

    public void SetResult(SessionResult result)
    {
        _stars.SetText(result.Stars.ToString());
        _coins.SetText(result.Coins.ToString());
    }
}
