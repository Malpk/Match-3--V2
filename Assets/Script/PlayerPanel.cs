using UnityEngine;
using TMPro;

public class PlayerPanel : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerState _bind;
    [SerializeField] private TextMeshProUGUI _name;

    public void Bind(PlayerState player)
    {
        _bind = player;
        if (_bind)
            _name.SetText(_bind.Name);
        else
            _name.SetText("Неизвестный");
    }
}
