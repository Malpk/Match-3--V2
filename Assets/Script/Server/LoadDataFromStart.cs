using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LoadDataFromStart : MonoBehaviour
{
    public string path;
    private NewServerData newServerData;

    void Awake()
    {
        newServerData = JsonUtility.FromJson<NewServerData>(File.ReadAllText(path));
        GameObject.FindObjectOfType<NetworkManager>().networkAddress = newServerData.ip;
        GameObject.FindObjectOfType<TelepathyTransport>().port = newServerData.port;
        if(!File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

[System.Serializable]
public class NewServerData
{
    public string ip;
    public ushort port;
}