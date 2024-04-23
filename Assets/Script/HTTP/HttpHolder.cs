using UnityEngine;
using System.Net.Http;

public class HttpHolder : MonoBehaviour
{
    [Header("Server")]
    [SerializeField] private string _adress;

    private HttpClient _client = new HttpClient();

    private void Reset()
    {
        _adress = "http://127.0.0.1:5000";
    }

    public async void SendGetMessange(string messange, System.Action<string> action)
    {
        using HttpResponseMessage response = await _client.GetAsync($"{_adress}/{messange}");
        string content = await response.Content.ReadAsStringAsync();
        action(content);
    }
}
