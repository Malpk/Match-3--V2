using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class PlayerRoundState : MonoBehaviour
{
    [SerializeField] private int _score;
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _sckoreText;
    [Header("Events")]
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private UnityEvent _onExit;

    private void Start()
    {
        _sckoreText.SetText("0");
    }

    public void Enter()
    {
        _onEnter.Invoke();
    }
    public void Exit()
    {
        _onExit.Invoke();
    }

    public void AddScore(int score)
    {
        _score += score;
        _sckoreText.SetText(_score.ToString());
    }
}
