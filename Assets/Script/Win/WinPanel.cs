using UnityEngine;
using System.Collections;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private float _moveDelte;
    [Header("Reference")]
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private CanvasGroup _result;
    [SerializeField] private GameObject _button;
    [SerializeField] private CharacterPanel _enemy;
    [SerializeField] private CharacterPanel _player;

    private Coroutine _corotine;

    public void SetMode(float alpha)
    {
        _canvas.alpha = alpha;
        _canvas.blocksRaycasts = alpha >= 1f;
    }

    public void Show(PlayerState player, PlayerState enemy)
    {
        _canvas.alpha = 0;
        _result.alpha = 0;
        _player.Hide();
        _enemy.Hide();
        _player.SetPlayer(player);
        _enemy.SetPlayer(enemy);
        _canvas.LeanAlpha(1, 0.2f).setOnComplete(() =>
        {
            if (_corotine != null)
                StopCoroutine(_corotine);
            _corotine = StartCoroutine(MoveTOPanel(_moveDelte));
            _result.LeanAlpha(1, 0.5f);
        });
    }

    private IEnumerator MoveTOPanel(float delte)
    {
        _player.Show(delte);
        _enemy.Show(delte);
        yield return new WaitWhile(() => !_player.IsShow || !_enemy.IsShow);
        _button.SetActive(true);
        _corotine = null;
    }
}
