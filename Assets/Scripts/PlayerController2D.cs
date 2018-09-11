using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerController2D : MonoBehaviour
{
    private CharacterController2D _controller;

    public SpriteRenderer magneticRenderer;
    public SpriteRenderer spriteRenderer;
    
    private Animator _animator;

    public float speed = 6.0f;
    public float maxJumpHeight = 4.0f;
    public float minJumpHeight = 1.0f;
    public float jumpTime = 0.4f;

    public float accelerationTimeAir = 0.2f;
    public float accelerationTimeGrounded = 0.1f;

    public float floatSpeed = 1.5f;
    public int omniDirectionalRayCount = 36;
    public int omniDirectionalRayLength = 10;

    private float _gravity;
    private float _maxJumpVelocity;
    private float _minJumpVelocity;

    private Vector2 _velocity;
    private float _velocityXSmoothing;

    private bool _floating;
    private Vector2 _floatDirection;

    public AudioClip floatClip;
    public AudioClip hurtClip;

    private AudioSource _audioSource;

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _gravity = (-2 * maxJumpHeight) / (jumpTime * jumpTime);
        
        _maxJumpVelocity = Mathf.Abs(_gravity) * jumpTime;
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * minJumpHeight);

        _floating = false;
        _floatDirection = Vector2.zero;

        Debug.Log("Gravity: " + _gravity);
        Debug.Log("Jump velocity: " + _maxJumpVelocity);
    }

    void Start()
    {
        Respawn(true);
    }

    void Update()
    {
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var jump = Input.GetButtonDown("Jump");

        Move(input, jump);
    }

    public void Move(Vector2 input, bool jumpDown)
    {
        var scale = spriteRenderer.transform.localScale;
        scale.x = _controller._facingDirection >= 0 ? 1 : -1;
        spriteRenderer.transform.localScale = scale;

        if (jumpDown)
        {
            if (!_floating)
            {
                var floatX = input.x != 0 ? Mathf.Sign(input.x) : 0;
                _floatDirection = new Vector2(floatX, 1.0f);
                _floatDirection.Normalize();
                _floating = true;

                magneticRenderer.enabled = true;

                PlaySound(floatClip);
            }
            else
            {
                var hasHit = false;
                var closestHit = new RaycastHit2D();

                for (int i = 0; i < omniDirectionalRayCount; i++)
                {
                    var rayOrigin = transform.position;
                    var rayDirection = Vector2.up.Rotate((360.0f / omniDirectionalRayCount) * i);

                    Debug.DrawRay(rayOrigin, rayDirection * omniDirectionalRayLength, Color.red);

                    var hit = Physics2D.Raycast(rayOrigin, rayDirection, omniDirectionalRayLength, _controller.collisionMask);
                    if (hit)
                    {
                        if (!hasHit || hit.distance < closestHit.distance)
                        {
                            closestHit = hit;
                            hasHit = true;
                        }
                    }
                }

                if (hasHit && closestHit.distance < omniDirectionalRayLength)
                {
                    var localRotation = transform.localRotation.eulerAngles;

                    var newRotationZ = Vector2.SignedAngle(Vector2.right, closestHit.normal) - 90;
                    if (newRotationZ != localRotation.z)
                        transform.position += (Vector3)closestHit.normal.normalized * 0.5f;

                    localRotation.z = newRotationZ;
                    transform.localRotation = Quaternion.Euler(localRotation.x, localRotation.y, localRotation.z);

                    _floating = false;
                    _floatDirection = Vector2.zero;

                    magneticRenderer.enabled = false;

                    PlaySound(floatClip);
                }
            }
        }

        var running = input.x != 0;
        var jumping = _velocity.y != 0;

        _animator.SetBool("running", running);
        _animator.SetBool("jumping", jumping);

        var targetVelocityX = input.x * speed;
        var smoothTime = _controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAir;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, smoothTime);

        if (!_floating)
        {
            _velocity.y += _gravity * Time.deltaTime;
        }
        else
        {
            _velocity = floatSpeed * _floatDirection;
        }

        if (!_floating && _velocity.x != 0)
        {
            _controller.CalculateRaySpacing();
            _controller.UpdateRaycastOrigins();

            var down = Vector2.down.Rotate(transform.localRotation.eulerAngles.z);
            var facingDirection = Mathf.Sign(_velocity.x);
            var rayOrigin = facingDirection < 0 ? _controller._raycastOrigins.bottomLeft : _controller._raycastOrigins.bottomRight;
            var rayDirection = down;
            var rayLength = 0.5f;

            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

            var hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, _controller.collisionMask);
            if (!hit)
            {
                _velocity.x = 0;
            }
        }

        _controller.Move(_velocity * Time.deltaTime, input);

        if (_controller.collisions.above || _controller.collisions.below)
        {
            if (_controller.collisions.slidingDownMaxSlope)
            {
                _velocity.y += _controller.collisions.slopeNormal.y * -_gravity * Time.deltaTime;
            }
            else
            {
                _velocity.y = 0;
            }
        }

        PrintDebugInfo(input);
    }

    public void Respawn(bool start = false)
    {
        var playerSpawns = GameObject.FindObjectsOfType<PlayerSpawnController>();
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            if (playerSpawns[i].active)
            {
                transform.position = playerSpawns[i].transform.position;
                transform.localRotation = Quaternion.Euler(0, 0, 0);

                _floating = false;
                _floatDirection = Vector2.zero;

                magneticRenderer.enabled = false;

                if (!start)
                    PlaySound(hurtClip);

                break;
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();

        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }

    private void PrintDebugInfo(Vector2 input)
    {
        var debugInfo = new StringBuilder();
        debugInfo.AppendLine("Player Info");

        debugInfo.AppendFormat("input: {0}", input);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("velocity: {0}", _velocity);
        debugInfo.AppendLine();

        var collisions = new List<string>();
        if (_controller.collisions.left) collisions.Add("left");
        if (_controller.collisions.above) collisions.Add("above");
        if (_controller.collisions.right) collisions.Add("right");
        if (_controller.collisions.below) collisions.Add("below");
        if (collisions.Count == 0) collisions.Add("none");

        debugInfo.AppendFormat("collisions: {0}", string.Join(", ", collisions.ToArray()));
        debugInfo.AppendLine();

        debugInfo.AppendFormat("climbing slope: {0}", _controller.collisions.climbingSlope);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("descending slope: {0}", _controller.collisions.descendingSlope);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("sliding slope: {0}", _controller.collisions.slidingDownMaxSlope);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("slope angle: {0}", _controller.collisions.slopeAngle);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("slope normal: {0}", _controller.collisions.slopeNormal);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("falling through: {0}", _controller.collisions.fallingThroughPlatform);
        debugInfo.AppendLine();

        DebugEx.ClearStatic();
        DebugEx.LogStatic(debugInfo.ToString());
    }
}