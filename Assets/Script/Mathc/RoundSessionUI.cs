using UnityEngine;
using UnityEngine.UI;

public class RoundSessionUI : MonoBehaviour
{
    [SerializeField] private Color _playerColor;
    [SerializeField] private Color _enemyColor;
    [Header("Reference")]
    [SerializeField] private Image _field;
    [SerializeField] private Animator[] _animators;

    public void SetRound(int round)
    {
        for (int i = 0; i < _animators.Length; i++)
        {
            _animators[i].SetBool("active", i < round);
        }
    }

    public void Switch(bool player)
    {
        _field.color = player ? _playerColor : _enemyColor;
    }

    public void SetProgress(float progress)
    {
        _field.fillAmount = progress;
    }

}
