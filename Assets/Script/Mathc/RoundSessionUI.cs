using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RoundSessionUI : NetworkBehaviour
{
    [SerializeField] private Color _playerColor;
    [SerializeField] private Color _enemyColor;
    [Header("Reference")]
    [SerializeField] private Image _field;
    [SerializeField] private Animator[] _animators;
    [SerializeField] private PlayerPanel _panelPlayer;
    [SerializeField] private PlayerPanel _panelEnemy;

    public PlayerPanel Player => _panelPlayer;
    public PlayerPanel Enemy => _panelEnemy;


    public void SetRound(int round)
    {
        for (int i = 0; i < _animators.Length; i++)
        {
            _animators[i].SetBool("active", i < round);
        }
        CliestSetRound(round);
    }

    [ClientRpc]
    private void CliestSetRound(int round)
    {
        for (int i = 0; i < _animators.Length; i++)
        {
            _animators[i].SetBool("active", i < round);
        }
    }



    [ClientRpc]
    public void Switch(PlayerState player)
    {
        _field.color = player.gameObject == NetworkClient.localPlayer.gameObject ?
            _playerColor : _enemyColor;
    }

    public void SetProgress(float progress)
    {
        _field.fillAmount = progress;
        SetProgressClient(progress);
    }

    [ClientRpc]
    private void SetProgressClient(float progress)
    {
        _field.fillAmount = progress;
    }

}
