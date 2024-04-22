using UnityEngine;
using TMPro;

public class WinMenu : MonoBehaviour
{
    [SerializeField] private bool _showWinPanel;
    [Header("Reference")]
    [SerializeField] private PlayerState _player;
    [SerializeField] private PlayerState _enemy;
    [SerializeField] private TextMeshProUGUI _playerScore;
    [SerializeField] private TextMeshProUGUI _enemyScore;
    [SerializeField] private WinPanel _winPanel;
    [SerializeField] private CanvasGroup _prewiew;
    [SerializeField] private ResultPanel _result;

    private void OnValidate()
    {
        if(_prewiew)
            _prewiew.alpha = _showWinPanel ? 0 : 1;
        _winPanel?.SetMode(_showWinPanel ? 1 : 0);
    }

    private void Start()
    {
        Show(_player, _enemy, new SessionResult(7, 15));
    }

    public void Show(PlayerState player, PlayerState enemy, SessionResult session)
    {
        _result.SetResult(session);
        _prewiew.alpha = 0;
        _prewiew.LeanAlpha(1, 1.5f).setOnComplete(() => _winPanel.Show(player, enemy));
        _winPanel.SetMode(0);
        _playerScore.SetText(player.Score.ToString());
        _enemyScore.SetText(enemy.Score.ToString());
        gameObject.SetActive(true);
    }

}
