using UnityEngine;
using Mirror;

public class Server : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("AddPlayer");
    }
}
