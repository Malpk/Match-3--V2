using UnityEngine;
using Mirror;

public class PlayerState : NetworkBehaviour 
{
    [SerializeField] private string _name;

    public string Name => _name;

     public void SetNick(string nick)
    {
        _name = nick;
        SetServerData(_name);
    }

    [Command]
    private void SetServerData(string nick)
    {
        _name = nick;
    }

}
