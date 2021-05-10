using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Drum : MonoBehaviour
{

    [SerializeField] GameObject _bullet;
    GameObject[] _bullets = new GameObject[6];
    [SerializeField] GameObject _ejectCollision;
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
        for (var i = 0; i < _bullets.Length; i++)
            _bullets[i].GetComponent<Rigidbody>().isKinematic = false;
        _rotationVelocity = _maxRotationVelocity;
        // animations to open
    }

    public void Close()
    {
        _audioSource.PlayOneShot(_closeSound);
        _IsOpen = false;
        _ejectCollision.SetActive(false);
        // animations to close
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
    public void Shoot(byte _bullets, float _shootAnimationSpeed)
    {
        print("shoot");
        this.gameObject.transform.DOLocalRotateQuaternion(RotationForIndex(_bullets), _shootAnimationSpeed);
    }
    Quaternion RotationForIndex(int curIndex)
    {
        float angle = AngleForIndex(curIndex);
        return Quaternion.AngleAxis(angle, Vector3.left);
    }
    float AngleForIndex(int curIndex)
    {
        return 360.0f * ((float)curIndex / (float)6) + 25;
    }
}
