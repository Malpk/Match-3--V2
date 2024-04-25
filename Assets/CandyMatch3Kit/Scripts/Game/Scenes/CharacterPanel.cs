using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private bool _isWin;
    [SerializeField] private float _hideOffset;
    [Header("Reference")]
    [SerializeField] private Image _loseShadow;
    [SerializeField] private GameObject _winLable;
    [SerializeField] private RectTransform _holder;

    private Coroutine _corotine;
    private SessionResult _player;

    public bool IsShow { get; private set; } = false;

    private void Reset()
    {
        _hideOffset = 600;
        _holder = GetComponent<RectTransform>();
    }

    private void OnValidate()
    {
        SetMode(_isWin);
    }

    public void SetMode(bool win)
    {
        if(_loseShadow)
            _loseShadow.gameObject.SetActive(!win);
        _winLable?.SetActive(win);
    }

    public void Show(float moveDelte)
    {
        if (_corotine != null)
            StopCoroutine(_corotine);
        _corotine = StartCoroutine(MoveTOPanel(moveDelte));
    }

    public void Hide()
    {
        IsShow = false;
        SetPosition(_hideOffset);
    }

    private void SetPosition(float position)
    {
        if(_holder)
            _holder.anchoredPosition = new Vector2(position, _holder.anchoredPosition.y);
    }

    private IEnumerator MoveTOPanel(float delte)
    {
        while (_holder.anchoredPosition.x != 0)
        {
            SetPosition(Mathf.MoveTowards(_holder.anchoredPosition.x, 0, delte));
            yield return null;
        }
        _corotine = null;
        IsShow = true;
    }
}
