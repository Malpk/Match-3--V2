using UnityEngine;
using GameVanilla.Game.Common;
using GameVanilla.Game.Scenes;
using Mirror;

public class RoundSession : MonoBehaviour
{
    [SerializeField] private int _coundSwipe;
    [SerializeField] private int _roundCount;
    [SerializeField] private float _roundTime;
    [SerializeField] private Vector2Int _starsWin;
    [SerializeField] private Vector2Int _starsLose;
    [SerializeField] private Vector2Int _coinsWin;
    [Header("Reference")]
    [SerializeField] private GameBoard _board;
    [SerializeField] private GameScene _scene;
    [SerializeField] private RoundSessionUI _sessionUI;

    private PlayerState _player;
    private PlayerState _enemy;

    private int _count;
    private int _curretCount;
    private int _countRound;
    private bool _isPlayer;
    private float _curretProgress = 0f;
    private PlayerState _curretState;

    public event System.Action<float> OnRound;
    public event System.Action OnWin;

    private void Awake()
    {
        _board.OnScore += AddScore;
        _board.OnSwipeStart += OnSwipeStart;
        _board.OnSwipeStop += OnSwipeStop;
    }

    private void OnDestroy()
    {
        _board.OnScore -= AddScore;
        _board.OnSwipeStop -= OnSwipeStop;
        _board.OnSwipeStart -= OnSwipeStart;
    }

    public void SetPlayer(PlayerState player, PlayerState enemy)
    {
        _player = player;
        _enemy = enemy;
        _sessionUI.Player.Bind(_player.Name);
        _sessionUI.Enemy.Bind(_enemy.Name);
        _sessionUI.Player.SetScore(_player.Score);
        _sessionUI.Enemy.SetScore(_enemy.Score);
    }


    public void StartGame()
    {
        enabled = true;
        _isPlayer = 1 == Random.Range(0, 2);
        SwitchPlayer();
        _curretState.Enter();
    }

    public void StopGame()
    {
        enabled = false;
        _curretProgress = 0;
        _count = 0;
        _coundSwipe = 0;
    }

    private void OnSwipeStart()
    {
        enabled = false;
        _curretProgress = 0;
        _sessionUI.SetProgress(1f - _curretProgress);
    }

    private void OnSwipeStop()
    {
        _curretCount++;
        if (_curretCount >= _coundSwipe)
        {
            _curretCount = 0;
            if (NextRound())
                SwitchPlayer();
            else
                CompliteGame();
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
            if (NextRound())
                SwitchPlayer();
            else
                CompliteGame();
        }
        _sessionUI.SetProgress(1f - _curretProgress);
    }

    private void AddScore(int score)
    {
        _curretState.AddScore(score);
        if (_curretState == _player)
        {
            _sessionUI.Player.SetScore(_curretState.Score);
        }
        else
        {
            _sessionUI.Enemy.SetScore(_curretState.Score);
        }
    }

    private void SwitchPlayer()
    {
        _curretState?.Exit();
        _curretState = _isPlayer ? _player : _enemy;
        _curretState.Enter();
        _sessionUI.Switch(_curretState);
        _isPlayer = !_isPlayer;
    }

    private bool NextRound()
    {
        _count++;
        if (_count >= 2)
        {
            _countRound++;
                    _sessionUI.SetRound(_countRound);
            _count = 0;
        }

        return _countRound <= _roundCount;
    }


    private void CompliteGame()
    {
        OnWin?.Invoke();
        StopGame();
        _player.SetWin(_player.Score > _enemy.Score);
        _enemy.SetWin(_enemy.Score > _player.Score);
        _player.CompliteGame(_enemy, GetSession(_player.IsWin));
        _enemy.CompliteGame(_player, GetSession(_enemy.IsWin));
    }

    private SessionResult GetSession(bool win)
    {
        var coints = win ? Random.Range(_coinsWin.x, _coinsWin.y) : Random.Range(0, 3);
        var stars = win ? Random.Range(_starsWin.x, _starsWin.y) :
            Random.Range(_starsLose.x, _starsLose.y);
        return new SessionResult(stars, coints);
    }

}
