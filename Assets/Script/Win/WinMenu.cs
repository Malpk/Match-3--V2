using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class WinMenu : NetworkBehaviour
{
    [SerializeField] private string _mainMenuID;
    [SerializeField] private bool _showWinPanel;
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI _playerScore;
    [SerializeField] private TextMeshProUGUI _enemyScore;
    [SerializeField] private WinPanel _winPanel;
    [SerializeField] private CanvasGroup _prewiew;
    [SerializeField] private ResultPanel _result;

    private void Reset()
    {
        _mainMenuID = "MainMenu";
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_prewiew)
            _prewiew.alpha = _showWinPanel ? 0 : 1;
        _winPanel?.SetMode(_showWinPanel ? 1 : 0);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(_mainMenuID);
    }

    [TargetRpc]
    public void Show(NetworkConnectionToClient client, string json)
    {
        var player = JsonUtility.FromJson<SessionResult>(json);
        _result.SetResult(player);
        _prewiew.alpha = 0;
        _prewiew.LeanAlpha(1, 1.5f).setOnComplete(() => _winPanel.Show(player));
        _winPanel.SetMode(0);
        gameObject.SetActive(true);
    }

    [TargetRpc]
    public void UpdateScore(NetworkConnectionToClient client, int player, int enemy)
    {
        _playerScore.SetText(player.ToString());
        _enemyScore.SetText(enemy.ToString());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
