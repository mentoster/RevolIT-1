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

    [SerializeField]
    float _maxRotationVelocity = 3;
    float _rotationVelocity;

    bool _IsOpen = false;
    bool _stopSpin = false;

    Quaternion rotationTarget;

    AudioSource _audioSource;
    private void Start()
    {

        _audioSource = gameObject.GetComponent<AudioSource>();
        GenerateBullets();
    }
    void FixedUpdate()
    {

        if (_rotationVelocity > 0 && _IsOpen)
        {
            _rotationVelocity -= _maxRotationVelocity / 250;
            transform.Rotate(_rotationVelocity, 0, 0);
        }
        if (_stopSpin && transform.localRotation.x < -0.2f && transform.localRotation.x > -0.3f)
        {
            print($"43. Drum -> transform.localRotation.x : {transform.localRotation.x}");
            _IsOpen = false;
            _stopSpin = false;
            gameObject.transform.DOLocalRotateQuaternion(RotationForIndex(0), 1);
        }

    }

    public void Open()
    {
        _ejectCollision.SetActive(true);
        _audioSource.PlayOneShot(_openSound);
        _audioSource.PlayOneShot(_rotateSound);
        _IsOpen = true;
        for (var i = 0; i < _bullets.Length; i++)
            foreach (var bullet in _bullets[i].transform.parent.gameObject.GetComponentsInChildren<Rigidbody>())
                bullet.isKinematic = false;


        _rotationVelocity = _maxRotationVelocity;
        // spin animations
        StartCoroutine(StopAnimation());
    }
    IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(2f);
        _stopSpin = true;
    }

    public void Close()
    {
        _audioSource.PlayOneShot(_closeSound);
        _ejectCollision.SetActive(false);
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
        gameObject.transform.DOLocalRotateQuaternion(RotationForIndex(_bullets), _shootAnimationSpeed);
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
