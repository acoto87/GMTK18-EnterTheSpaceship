using UnityEngine;
using UnityEngine.UI;

public class TextAnimController : MonoBehaviour
{
    public Color color1, color2;
    public float speed = 1.0f;

    private Text _text;
    private float _t;
    private bool _from1to2;

    void Awake()
    {
        _text = GetComponent<Text>();
        _t = 0;
        _from1to2 = true;
    }

    void Update()
    {
        if (_from1to2)
        {
            _text.color = Color.Lerp(color1, color2, _t);
            _t += speed * Time.deltaTime;
            if (_t >= 1)
                _from1to2 = false;
        }
        else
        {
            _text.color = Color.Lerp(color2, color1, _t);
            _t -= speed * Time.deltaTime;
            if (_t <= 0)
                _from1to2 = true;
        }
    }
}
