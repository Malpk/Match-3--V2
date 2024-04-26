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
    [SerializeField] private WinMenu _winMenu;
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

    public bool IsComplite => !enabled;

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

    public void SetPlayer(PlayerState player, PlayerState enemy, bool bot)
    {
        _player = new PlayerSesion(player);
        _enemy = new PlayerSesion(enemy, bot);
        _curretState = _player;
        _sessionUI.Player.SetScore(0);
        _sessionUI.Enemy.SetScore(0);
        UpdateScore(_player, _enemy);
        UpdateScore(_enemy, _player);
    }

    public void Reconect(PlayerState player)
    {
        if (_player.Adress == player.Adress && !_player.Player)
        {
            _player.Reconect(player);
        }
        else if (_enemy.Adress == player.Adress)
        {
            _enemy.Reconect(player);
        }
        _sessionUI.Switch(_curretState.Player);
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
        UpdateScore(_player, _enemy);
        UpdateScore(_enemy, _player);
    }

    private void UpdateScore(PlayerSesion player, PlayerSesion enemy)
    {
        if (player.Player && !player.IsBot)
        {
            Debug.Log("upadet score");
            _winMenu.UpdateScore(player.Player.netIdentity.connectionToClient,
                player.Score, enemy.Score);
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
        SaveResult(_player, _enemy);
        if(!_enemy.IsBot)
            SaveResult(_enemy, _player);
    }

    private void SaveResult(PlayerSesion player, PlayerSesion enemy)
    {
        var win = player.Player ? player.Score > enemy.Score : false;
        var result = GetPlayerResult(player, enemy.Login, win);
        if (player.Player)
        {
            _winMenu.Show(player.Player.netIdentity.connectionToClient, JsonUtility.ToJson(result));
        }
        OnWin?.Invoke($"{player.Adress}/{result.Stars}/{result.Coins}");
    }

    private SessionResult GetPlayerResult(PlayerSesion session, string enemy, bool win)
    {
        if (session.Player)
        {
            var result = GetSession(win);
            result.Player = session.Login;
            result.Score = session.Score;
            result.Enemy = enemy;
            result.Win = win;
            return result;
        }
        var result1 = GetSession(false);
        result1.Player = session.Login;
        return result1;
    }

    private SessionResult GetSession(bool win)
    {
        var coints = win ? Random.Range(_coinsWin.x, _coinsWin.y) : Random.Range(0, 3);
        var stars = win ? Random.Range(_starsWin.x, _starsWin.y) :
            Random.Range(_starsLose.x, _starsLose.y);
        return new SessionResult(stars, coints);
    }

}
