using UnityEngine;
using GameVanilla.Game.Common;
using GameVanilla.Game.Scenes;


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

    private PlayerSesion _player;
    private PlayerSesion _enemy;

    private int _count;
    private int _curretCount;
    private int _countRound;
    private bool _isPlayer;
    private float _curretProgress = 0f;
    private PlayerSesion _curretState;

    public event System.Action<float> OnRound;
    public event System.Action<string> OnWin;

    public bool IsComplite => enabled;

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
        _player = new PlayerSesion(player);
        _enemy = new PlayerSesion(enemy);
        _curretState = _player;
    }

    public void Reconect(PlayerState player)
    {
        if (_player.Adress == player.Adress)
        {
            _player.Reconect(player);
        }
        else if (_enemy.Adress == player.Adress)
        {
            _enemy.Reconect(player);
        }
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
            if(!Next())
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

    private bool Next()
    {
        if (NextRound())
           return SwitchPlayer();
        return false;
    }

    private bool SwitchPlayer()
    {
        if (_curretState.Player)
        {
            _curretState.Exit();
            _curretState = _isPlayer ? _player : _enemy;
            _curretState.Enter();
            _sessionUI.Switch(_curretState.Player);
            _isPlayer = !_isPlayer;
            return true;
        }
        return false;
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
        StopGame();
        var playerWin = _player.Score > _enemy.Score;
        var resultP1 = SetPlayerResult(_player.Player, playerWin);
        var resultP2 = SetPlayerResult(_enemy.Player,!playerWin);
        OnWin?.Invoke($"{JsonUtility.ToJson(resultP1)} ::: {JsonUtility.ToJson(resultP2)} ");
    }

    private SessionResult SetPlayerResult(PlayerState player, bool win)
    {
        if (player)
        {
            player.SetWin(win);
            var result = GetSession(win);
            player.CompliteGame(player, result);
            return result;
        }
        return GetSession(false);
    }

    private SessionResult GetSession(bool win)
    {
        var coints = win ? Random.Range(_coinsWin.x, _coinsWin.y) : Random.Range(0, 3);
        var stars = win ? Random.Range(_starsWin.x, _starsWin.y) :
            Random.Range(_starsLose.x, _starsLose.y);
        return new SessionResult(stars, coints);
    }

}
