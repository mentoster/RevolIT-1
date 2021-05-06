using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drum : MonoBehaviour
{

    [SerializeField] GameObject _bullet;
    GameObject[] _bullets = new GameObject[6];
    [SerializeField] GameObject _ejectCollision;
    [Header("Sounds")]
    [SerializeField] AudioClip _openSound;
    [SerializeField] AudioClip _rotateSound;
    [SerializeField] AudioClip _closeSound;
    [SerializeField] Transform[] _bulletsPositions;
    Rigidbody m_Rigidbody;
    [SerializeField]
    float _maxRotationVelocity = 3;
    float _rotationVelocity;

    bool _IsOpen = false;


    AudioSource _audioSource;
    private void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        GenerateBullets();
    }
    private void FixedUpdate()
    {
        if (_IsOpen)
        {
            _rotationVelocity -= _maxRotationVelocity / 200;
            transform.Rotate(_rotationVelocity, 0, 0);
        }
    }
    public void Open()
    {
        _ejectCollision.SetActive(true);
        _audioSource.PlayOneShot(_openSound);
        _audioSource.PlayOneShot(_rotateSound);
        _IsOpen = true;
        _rotationVelocity = _maxRotationVelocity;
        // animations to open
        transform.parent.transform.position = new Vector3(transform.position.x, transform.position.y - 0.005f, transform.position.z);
    }

    public void Close()
    {
        _audioSource.PlayOneShot(_closeSound);
        _IsOpen = false;
        _ejectCollision.SetActive(false);
        // animations to close
        transform.parent.transform.position = new Vector3(transform.position.x, transform.position.y + 0.005f, transform.position.z);
        GenerateBullets();
    }
    void GenerateBullets()
    {
        for (var i = 0; i < _bulletsPositions.Length; i++)
        {
            _bullets[i] = Instantiate(_bullet, _bulletsPositions[i].position, _bulletsPositions[i].rotation);
            _bullets[i].transform.parent = _bulletsPositions[i];
        }

    }
}
