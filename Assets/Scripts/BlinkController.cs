using UnityEngine;

public class BlinkController : MonoBehaviour
{
    public bool blink;
    public float showTime;
    public float hideTime;
    public float delay;

    private float _nextBlink;

    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        if (blink)
        {
            _nextBlink = Time.deltaTime + delay;
            Debug.Log("start blink at: " + _nextBlink);
        }
    }

    void Update()
    {
        if (blink)
        {
            if (Time.time >= _nextBlink)
            {
                if (_renderer != null)
                    _renderer.enabled = !_renderer.enabled;

                if (_collider != null)
                    _collider.enabled = !_collider.enabled;

                if (_renderer.enabled)
                    _nextBlink = Time.time + showTime;
                else
                    _nextBlink = Time.time + hideTime;
            }
        }
    }
}
