using UnityEngine;
using GameVanilla.Game.Common;
using GameVanilla.Game.Scenes;
using Mirror;

public class RoundSession : MonoBehaviour
{
    [SerializeField] private int _coundSwipe;
    [SerializeField] private int _roundCount;
    [SerializeField] private float _roundTime;
    [Header("Reference")]
    [SerializeField] private PlayerState _bot;
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
        _scene.BlcokInput(true);
        _isPlayer = 1 == Random.Range(0, 2);
        SwitchPlayer();
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
        _count++;
        if (_count >= 2)
        {
            _count = 0;
            _countRound++;
            _sessionUI.SetRound(_countRound);
            if (_countRound >= _roundCount)
                OnWin?.Invoke();
        }
    }


}
