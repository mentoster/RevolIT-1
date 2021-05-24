using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] AudioClip groundSound;
    AudioSource _audioSource;


    private void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }
    public void DeleteTimer(float time)
    {
        Destroy(gameObject,time);
    }
    private void OnCollisionEnter(Collision other)
    {
        _audioSource.PlayOneShot(groundSound);

    }

}
