using UnityEngine;
using System.Net.Http;

public class HttpHolder : MonoBehaviour
{
    [SerializeField] private bool _isDebugMode;
    [Header("Server")]
    [SerializeField] private bool _isServer;
    [SerializeField] private string _adressServera;

    private HttpClient _client = new HttpClient();

    public event System.Action<string> OnGetMessange;

    private void Reset()
    {
        _adressServera = "127.0.0.1:5000";
    }

    public async void SendGetMessange(string adress, System.Action<string> action)
    {
        if (_isDebugMode)
            Debug.Log("http://" + $"{_adressServera}/{adress}");
        using HttpResponseMessage response = await _client.GetAsync("http://" + $"{_adressServera}/{adress}");
        string content = await response.Content.ReadAsStringAsync();
        action(content);
    }
}
