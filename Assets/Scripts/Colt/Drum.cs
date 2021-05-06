using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drum : MonoBehaviour
{

    [SerializeField] GameObject _bullet;
    GameObject[] _bullets = new GameObject[6];
    [SerializeField] GameObject _ejectCollision;
    [SerializeField] float  _animPos=0.015f;
    [SerializeField] Transform[] _bulletsPositions;
    [Header("Sounds")]
    [SerializeField] AudioClip _openSound;
    [SerializeField] AudioClip _rotateSound;
    [SerializeField] AudioClip _closeSound;
    Rigidbody m_Rigidbody;
    [SerializeField]
    float _maxRotationVelocity = 3;
    float _rotationVelocity;

    bool _IsOpen = false;
    Transform _parentTransfrorm;


    AudioSource _audioSource;
    private void Start()
    {
        _parentTransfrorm = transform.parent.transform;
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
        _parentTransfrorm.localPosition = new Vector3(_parentTransfrorm.localPosition.x, _parentTransfrorm.localPosition.y - _animPos, _parentTransfrorm.localPosition.z);
    }

    public void Close()
    {
        _audioSource.PlayOneShot(_closeSound);
        _IsOpen = false;
        _ejectCollision.SetActive(false);
        // animations to close
        _parentTransfrorm.localPosition = new Vector3(_parentTransfrorm.localPosition.x, _parentTransfrorm.localPosition.y + _animPos, _parentTransfrorm.localPosition.z);
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
