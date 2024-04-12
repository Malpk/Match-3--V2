using UnityEngine;

public class MatchCell : MonoBehaviour
{
    [SerializeField] private Vector2 _position;
    [SerializeField] private Vector2Int _localPosition;

    public Vector2 Position => _position;
    public Vector2Int LocalPosition => _localPosition;

    public void SetPosition(Vector2 position, Vector2Int local)
    {
        _position = position;
        _localPosition = local;
    }
}
