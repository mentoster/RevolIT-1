using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drum : MonoBehaviour
{

    [SerializeField] GameObject _bullet;
    [Header("Sounds")]
    [SerializeField] AudioClip _closeSound;
    [SerializeField] AudioClip _openSound;
    [SerializeField] Transform[] _bulletsPositions;
    AudioSource _audioSource;
    private void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        GenerateBullets();
    }
    public void Open()
    {
        _audioSource.PlayOneShot(_openSound);
        // animations to open
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.02f, transform.position.z);
    }
    public void Close()
    {
        _audioSource.PlayOneShot(_closeSound);
        // animations to close
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, transform.position.z);
        GenerateBullets();
    }
    void GenerateBullets()
    {
        foreach (var transform in _bulletsPositions)
            Instantiate(_bullet, transform.position, transform.rotation);
    }
}
