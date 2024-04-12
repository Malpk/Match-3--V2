using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class PlayerRoundState : MonoBehaviour
{
    [SerializeField] private int _score;
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _sckoreText;
    [Header("Events")]
    [SerializeField] private UnityEvent _onUpdate;

    public void Enter()
    {
        _onUpdate.Invoke();
    }

    public void AddScore(int score)
    {
        _score += score;
        _sckoreText.SetText(_score.ToString());
    }
}
