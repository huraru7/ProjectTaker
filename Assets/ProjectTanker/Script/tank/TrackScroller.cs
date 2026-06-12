using UnityEngine;

public class TrackScroller : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _trackRenderer;
    [SerializeField] private Rigidbody2D    _rb;
    [SerializeField] private float          _scrollSpeed = 0.8f;

    private Material _mat;
    private float    _offset;

    void Start() => _mat = _trackRenderer.material;

    void Update()
    {
        _offset += _rb.linearVelocity.magnitude * _scrollSpeed * Time.deltaTime;
        _mat.mainTextureOffset = new Vector2(0f, _offset);
    }

    void OnDestroy() => Destroy(_mat);
}