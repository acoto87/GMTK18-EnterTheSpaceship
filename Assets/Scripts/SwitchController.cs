using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public Sprite OnSprite;
    public Sprite OffSprite;

    public bool isOn;

    public DoorController[] doors;

    private SpriteRenderer _renderer;
    private AudioSource _audioSource;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isOn)
        {
            _renderer.sprite = OnSprite;
        }
        else
        {
            _renderer.sprite = OffSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].Open();
            }

            _audioSource.Play();

            isOn = true;
        }
    }
}
