using UnityEngine;
using GameVanilla.Game.Common;

public class RoundSession : MonoBehaviour
{
    [SerializeField] private int _coundSwipe;
    [SerializeField] private int _roundCount;
    [SerializeField] private float _roundTime;
    [Header("Reference")]
    [SerializeField] private GameBoard _board;
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
        enabled = true;
    }

    private void Start()
    {
        _isPlayer = 1 == Random.Range(0, 2);
        SwitchPlayer();
    }

    private void Update()
    {
        _curretProgress += Time.deltaTime / _roundTime;
        if (_curretProgress >= 1f)
        {
            _curretCount = 0;
            _curretProgress = 0;
        }
        _uiPanel.SetProgress(1f - _curretProgress);
    }

    private void AddScore(int score)
    {
        _curretState.AddScore(score);
    }

    private void SwitchPlayer()
    {
        _uiPanel.Switch(_isPlayer);
        _curretState = _isPlayer ? _player : _enemy;
        _isPlayer = !_isPlayer;
        _uiPanel.Switch(_isPlayer);
    }


}
