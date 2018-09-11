using UnityEngine;

public class AutoPlayerConntroller : MonoBehaviour
{
    private PlayerController2D _player;

    private float _dir;
    private bool _jump;
    private float _timeForJump;
    private float _timeForMove;
    private bool _playerTookControl;

    void Awake()
    {
        _player = GetComponent<PlayerController2D>();
        _dir = 1;
    }

    private void Start()
    {
        _timeForMove = Time.time + 1.5f;
        _timeForJump = Time.time + 4.0f;
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetButtonDown("Jump"))
        {
            _playerTookControl = true;
        }

        if (!_playerTookControl)
        {
            if (Time.time >= _timeForJump)
            {
                _jump = true;
            }

            var input = Vector2.zero;
            if (Time.time >= _timeForMove)
            {
                input.x = _dir;
            }

            _player.Move(input, _jump);
            _jump = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (_playerTookControl) return;

        if (collider.CompareTag("MenuLeftCollider"))
        {
            _dir = 1;
            _timeForMove = Time.time + 2.0f;
        }
        else if(collider.CompareTag("MenuRightCollider"))
        {
            _dir = -1;
            _timeForMove = Time.time + 2.0f;
        }
        else if(collider.CompareTag("MenuTopCollider"))
        {
            _jump = true;
            _timeForJump = Time.time + 5;
        }
    }
}
