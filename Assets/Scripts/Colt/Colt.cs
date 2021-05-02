using System;
using UnityEngine;

public class Colt : MonoBehaviour
{
    [SerializeField] Byte _bullets = 6;
    Shoot _shoot;
    Reload _reload;
    AudioSource _audioSource;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _reload.Set(_bullets, _audioSource);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _bullets = _shoot.Use(_bullets);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            _bullets = _reload.Use(_bullets);
        }
    }

}
