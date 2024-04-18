using UnityEngine;

public class MachServer : MonoBehaviour
{
    [SerializeField] private Server _server;

    private void Start()
    {
        _server.StartServer();
    }
}
