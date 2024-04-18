using UnityEngine;
using GameVanilla.Game.Common;
using GameVanilla.Game.Scenes;

public class RoundSession : MonoBehaviour
{
    [SerializeField] private int _coundSwipe;
    [SerializeField] private int _roundCount;
    [SerializeField] private float _roundTime;
    [Header("Reference")]
    [SerializeField] private GameBoard _board;
    [SerializeField] private GameScene _scene;
    [SerializeField] private RoundSessionUI _uiPanel;
    [SerializeField] private PlayerRoundState _player;
    [SerializeField] private PlayerRoundState _enemy;

    private int _count;
    private int _curretCount;
    private int _countRound;
    private bool _isPlayer;
    private float _curretProgress = 0f;
    private PlayerRoundState _curretState;

    public event System.Action<float> OnRound;
    public event System.Action OnWin;

    private void Awake()
    {
        _scene.OnStartGame += OnStartGame;
        _board.OnScore += AddScore;
        _board.OnSwipeStart += OnSwipeStart;
        _board.OnSwipeStop += OnSwipeStop;
    }

    private void OnDestroy()
    {
        _scene.OnStartGame -= OnStartGame;
        _board.OnScore -= AddScore;
        _board.OnSwipeStop -= OnSwipeStop;
        _board.OnSwipeStart -= OnSwipeStart;
    }

    private void OnStartGame()
    {
        enabled = true;
        _scene.BlcokInput(true);
        _isPlayer = 1 == Random.Range(0, 2);
        SwitchPlayer();
    }

    private void OnSwipeStart()
    {
        enabled = false;
        _curretProgress = 0;
        _uiPanel.SetProgress(1f - _curretProgress);
    }

    private void OnSwipeStop()
    {
        _curretCount++;
        if (_curretCount >= _coundSwipe)
        {
            _curretCount = 0;
            SwitchPlayer();
        }
        enabled = true;
    }

    private void Update()
    {
        _curretProgress += Time.deltaTime / _roundTime;
        if (_curretProgress >= 1f)
        {
            _curretCount = 0;
            _curretProgress = 0;
            SwitchPlayer();
        }
        _uiPanel.SetProgress(1f - _curretProgress);
    }

    private void AddScore(int score)
    {
        _curretState.AddScore(score);
    }

    private void SwitchPlayer()
    {
        _curretState?.Exit();
        _uiPanel.Switch(_isPlayer);
        _curretState = _isPlayer ? _player : _enemy;
        _uiPanel.Switch(_isPlayer);
        _curretState.Enter();
        _isPlayer = !_isPlayer;
        _count++;
        if (_count >= 2)
        {
            _count = 0;
            _countRound++;
            _uiPanel.SetRound(_countRound);
            if (_countRound >= _roundCount)
                OnWin?.Invoke();
        }
    }


}
