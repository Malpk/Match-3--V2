using UnityEngine;
using System.Net;
using System.Net.Http;
using System.Collections;
using UnityEngine.Networking;

public class HttpHolder : MonoBehaviour
{
    [Header("Server")]
    [SerializeField] private string _adress;

    private HttpClient _client = new HttpClient();
    private HttpListener _lister = new HttpListener();

    private void Reset()
    {
        _adress = "http://127.0.0.1:5000";
    }

    private void Awake()
    {
        _lister.Prefixes.Add("http://localhost/8080/");
    }

    private void OnEnable()
    {
        _lister.Start();
    }


    private void OnDisable()
    {
        _lister.Stop();
    }

    void Start()
    {
        // ��������� ������
        StartCoroutine(GetHTML());
    }

    IEnumerator GetHTML()
    {
        // URL ��������, ������� �� ����� ���������
        string url = "http://example.com";

        // ���������� GET-������ � ������� �����
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        // ���������, ��� �� ������ �������� �������
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // �������� HTML-������� ������
            string htmlContent = request.downloadHandler.text;
            Debug.Log("HTML content: " + htmlContent);
        }
    }

    private void OnDestroy()
    {
        _lister.Close();
    }

    public async void SendGetMessange(string messange, System.Action<string> action)
    {
        using HttpResponseMessage response = await _client.GetAsync($"{_adress}/{messange}");
        string content = await response.Content.ReadAsStringAsync();
        action(content);
    }
}
