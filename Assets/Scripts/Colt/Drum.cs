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
    [SerializeField] AudioClip _closeSound;
    [SerializeField] Transform[] _bulletsPositions;
    Rigidbody m_Rigidbody;
    float _rotationVelocity = 3;
    bool _IsOpen = false;


    AudioSource _audioSource;
    private void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        GenerateBullets();
    }
    private void FixedUpdate()
    {
        if (_IsOpen)
        {
            _rotationVelocity -= 0.05f;
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(_rotationVelocity, 0, 0) * Time.fixedDeltaTime);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
        }
    }
    public void Open()
    {
        _ejectCollision.SetActive(true);
        _audioSource.PlayOneShot(_openSound);
        _IsOpen = true;
        _rotationVelocity = 1000;
        // animations to open
        transform.parent.transform.position = new Vector3(transform.position.x, transform.position.y - 0.02f, transform.position.z);
    }
    public void Close()
    {
        _audioSource.PlayOneShot(_closeSound);
        _IsOpen = false;
        _ejectCollision.SetActive(false);
        // animations to close
        transform.parent.transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, transform.position.z);
        GenerateBullets();
    }
    void GenerateBullets()
    {
        for (var i = 0; i < _bulletsPositions.Length; i++)
        {
            _bullets[i] = Instantiate(_bullet, _bulletsPositions[i].position, _bulletsPositions[i].rotation);
            _bullets[i].transform.parent = gameObject.transform;
        }

    }
}
