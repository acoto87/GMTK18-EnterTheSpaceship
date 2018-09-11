using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CircleCollider2D))]
public class DistanceAudioController : MonoBehaviour
{
    private AudioSource _audioSource;

    private bool _increaseVolume;
    private bool _decreaseVolume;
    private float _maxVolume;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _maxVolume = _audioSource.volume;
    }

    void Start()
    {
        _audioSource.volume = 0;    
    }

    void Update()
    {
        if (_increaseVolume)
        {
            _audioSource.volume += 0.5f * Time.deltaTime;
            _audioSource.volume = Mathf.Clamp(_audioSource.volume, 0, _maxVolume);
            if (_audioSource.volume >= _maxVolume)
                _increaseVolume = false;
        }
        else if(_decreaseVolume)
        {
            _audioSource.volume -= 1.5f * Time.deltaTime;
            _audioSource.volume = Mathf.Clamp(_audioSource.volume, 0, _maxVolume);
            if (_audioSource.volume <= 0)
                _decreaseVolume = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            _audioSource.Play();
            _increaseVolume = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            _audioSource.Stop();
            _decreaseVolume = true;
        }
    }
}
