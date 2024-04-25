using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UserAuto : MonoBehaviour
{
    public const string USERKEY = "userAuto"; 
  
    [Header("Server")]
    [SerializeField] private string _adress;
    [SerializeField] private string _login;
    [Header("Reference")]
    [SerializeField] private Button _applay;
    [SerializeField] private HttpHolder _holder;
    [Header("Events")]
    [SerializeField] private UnityEvent<string> _onLoadLogin;

    private bool _isReady = true;
    private string _newLogin;

    public UserData User { get; private set; }

    private void Awake()
    {
        _applay.onClick.AddListener(() =>
        {
            Replace(_newLogin);
            _applay.interactable = false;
        });
    }

    private void Start()
    {
        _onLoadLogin?.Invoke(_login);
        SetLogin(_login);
        Registrate();
    }

    public void SetLogin(string login)
    {
        _newLogin = login;
        _applay.interactable = _login != _newLogin && _isReady;
    }

    private void Registrate()
    {
        if (!PlayerPrefs.HasKey(USERKEY))
        {
            Replace(_login);
        }
        else
        {
            User = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString(USERKEY));
        }
    }

    private void Replace(string login)
    {
        if (_isReady)
        {
            _isReady = false;
            _applay.interactable = false;
           _login = login;
            _holder.SendGetMessange($"{_adress}/{_login}", (string content) =>
            {
                _isReady = true;
                _applay.interactable = _newLogin != login;
                PlayerPrefs.SetString(USERKEY, content);
                User = JsonUtility.FromJson<UserData>(content);
            });
        }
    }

}
